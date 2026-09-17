using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class RoyaltyService : IRoyaltyService
{
    private readonly IRoyaltyRepository _royaltyRepository;
    private readonly IUnidadeFranqueadaRepository _unidadeRepository;

    public RoyaltyService(IRoyaltyRepository royaltyRepository, IUnidadeFranqueadaRepository unidadeRepository)
    {
        _royaltyRepository = royaltyRepository;
        _unidadeRepository = unidadeRepository;
    }

    public async Task<RoyaltyRespostaDto> CalcularAsync(RoyaltyCalculoRequestDto dto)
    {
        var unidade = await _unidadeRepository.ObterPorIdAsync(dto.UnidadeFranqueadaId)
            ?? throw new KeyNotFoundException("Unidade franqueada não encontrada.");

        var inicioPeriodo = new DateTime(dto.AnoReferencia, dto.MesReferencia, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimPeriodo = inicioPeriodo.AddMonths(1);

        var faturamento = await _royaltyRepository.ObterFaturamentoUnidadeAsync(
            dto.UnidadeFranqueadaId, inicioPeriodo, fimPeriodo);

        var valorCalculado = faturamento * unidade.PercentualRoyalty / 100m;

        var royalty = await _royaltyRepository.ObterPorUnidadeEPeriodoAsync(
            dto.UnidadeFranqueadaId, dto.AnoReferencia, dto.MesReferencia);

        if (royalty is not null)
        {
            if (royalty.StatusPagamento == StatusPagamento.Pago)
            {
                throw new InvalidOperationException(
                    "Não é possível recalcular um royalty cujo pagamento já foi confirmado.");
            }

            royalty.FaturamentoBase = faturamento;
            royalty.PercentualAplicado = unidade.PercentualRoyalty;
            royalty.ValorCalculado = valorCalculado;
        }
        else
        {
            royalty = new Royalty
            {
                UnidadeFranqueadaId = dto.UnidadeFranqueadaId,
                UnidadeFranqueada = unidade,
                AnoReferencia = dto.AnoReferencia,
                MesReferencia = dto.MesReferencia,
                FaturamentoBase = faturamento,
                PercentualAplicado = unidade.PercentualRoyalty,
                ValorCalculado = valorCalculado,
                StatusPagamento = StatusPagamento.Pendente,
                DataVencimento = fimPeriodo.AddDays(9)
            };

            await _royaltyRepository.AdicionarAsync(royalty);
        }

        await _royaltyRepository.SalvarAlteracoesAsync();

        return MapearParaDto(royalty);
    }

    public async Task<ResultadoPaginadoDto<RoyaltyRespostaDto>> ListarAsync(
        int? unidadeId,
        int? anoReferencia,
        int? mesReferencia,
        StatusPagamento? statusPagamento,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        var (itens, total) = await _royaltyRepository.ListarAsync(
            unidadeId, anoReferencia, mesReferencia, statusPagamento, pagina, tamanhoPagina, ordenarPor, decrescente);

        return new ResultadoPaginadoDto<RoyaltyRespostaDto>
        {
            Itens = itens.Select(MapearParaDto).ToList(),
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<RoyaltyRespostaDto> AtualizarStatusAsync(int id, RoyaltyAtualizacaoStatusDto dto)
    {
        var royalty = await _royaltyRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Royalty não encontrado.");

        royalty.StatusPagamento = dto.StatusPagamento;
        royalty.DataPagamento = dto.StatusPagamento == StatusPagamento.Pago
            ? dto.DataPagamento ?? DateTime.UtcNow
            : null;

        await _royaltyRepository.SalvarAlteracoesAsync();

        return MapearParaDto(royalty);
    }

    private static RoyaltyRespostaDto MapearParaDto(Royalty royalty)
    {
        return new RoyaltyRespostaDto
        {
            Id = royalty.Id,
            UnidadeFranqueadaId = royalty.UnidadeFranqueadaId,
            UnidadeFranqueadaNome = royalty.UnidadeFranqueada.Nome,
            AnoReferencia = royalty.AnoReferencia,
            MesReferencia = royalty.MesReferencia,
            FaturamentoBase = royalty.FaturamentoBase,
            PercentualAplicado = royalty.PercentualAplicado,
            ValorCalculado = royalty.ValorCalculado,
            StatusPagamento = royalty.StatusPagamento,
            DataVencimento = royalty.DataVencimento,
            DataPagamento = royalty.DataPagamento
        };
    }
}
