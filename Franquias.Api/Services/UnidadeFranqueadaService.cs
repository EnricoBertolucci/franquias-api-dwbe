using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class UnidadeFranqueadaService : IUnidadeFranqueadaService
{
    private readonly IUnidadeFranqueadaRepository _unidadeRepository;
    private readonly IFranqueadoraRepository _franqueadoraRepository;

    public UnidadeFranqueadaService(
        IUnidadeFranqueadaRepository unidadeRepository,
        IFranqueadoraRepository franqueadoraRepository)
    {
        _unidadeRepository = unidadeRepository;
        _franqueadoraRepository = franqueadoraRepository;
    }

    public async Task<ResultadoPaginadoDto<UnidadeFranqueadaRespostaDto>> ListarAsync(
        string? nome,
        string? cidade,
        string? cnpj,
        string? responsavel,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        pagina = pagina < 1 ? 1 : pagina;
        tamanhoPagina = tamanhoPagina is < 1 or > 100 ? 10 : tamanhoPagina;

        var (itens, total) = await _unidadeRepository.ListarAsync(
            nome, cidade, cnpj, responsavel, pagina, tamanhoPagina, ordenarPor, decrescente);

        return new ResultadoPaginadoDto<UnidadeFranqueadaRespostaDto>
        {
            Itens = itens.Select(MapearParaDto).ToList(),
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<UnidadeFranqueadaRespostaDto> ObterPorIdAsync(int id)
    {
        var unidade = await _unidadeRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Unidade franqueada não encontrada.");

        return MapearParaDto(unidade);
    }

    public async Task<UnidadeFranqueadaRespostaDto> CriarAsync(UnidadeFranqueadaCriacaoDto dto)
    {
        if (await _franqueadoraRepository.ObterPorIdAsync(dto.FranqueadoraId) is null)
        {
            throw new KeyNotFoundException("Franqueadora não encontrada.");
        }

        if (await _unidadeRepository.ExisteCnpjAsync(dto.Cnpj))
        {
            throw new InvalidOperationException("Já existe uma unidade cadastrada com este CNPJ.");
        }

        var unidade = new UnidadeFranqueada
        {
            FranqueadoraId = dto.FranqueadoraId,
            Nome = dto.Nome,
            Cnpj = dto.Cnpj,
            Telefone = dto.Telefone,
            Logradouro = dto.Logradouro,
            Cidade = dto.Cidade,
            Estado = dto.Estado,
            Cep = dto.Cep,
            DataInicio = dto.DataInicio,
            PercentualRoyalty = dto.PercentualRoyalty,
            Situacao = SituacaoUnidade.Ativa
        };

        AtribuirResponsaveis(unidade, dto.Responsaveis);

        await _unidadeRepository.AdicionarAsync(unidade);
        await _unidadeRepository.SalvarAlteracoesAsync();

        return MapearParaDto(unidade);
    }

    public async Task<UnidadeFranqueadaRespostaDto> AtualizarAsync(int id, UnidadeFranqueadaAtualizacaoDto dto)
    {
        var unidade = await _unidadeRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Unidade franqueada não encontrada.");

        if (await _unidadeRepository.ExisteCnpjAsync(dto.Cnpj, id))
        {
            throw new InvalidOperationException("Já existe uma unidade cadastrada com este CNPJ.");
        }

        unidade.Nome = dto.Nome;
        unidade.Cnpj = dto.Cnpj;
        unidade.Telefone = dto.Telefone;
        unidade.Logradouro = dto.Logradouro;
        unidade.Cidade = dto.Cidade;
        unidade.Estado = dto.Estado;
        unidade.Cep = dto.Cep;
        unidade.DataInicio = dto.DataInicio;
        unidade.PercentualRoyalty = dto.PercentualRoyalty;

        unidade.Responsaveis.Clear();
        AtribuirResponsaveis(unidade, dto.Responsaveis);

        await _unidadeRepository.SalvarAlteracoesAsync();

        return MapearParaDto(unidade);
    }

    public async Task InativarAsync(int id)
    {
        var unidade = await _unidadeRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Unidade franqueada não encontrada.");

        unidade.Situacao = SituacaoUnidade.Inativa;
        await _unidadeRepository.SalvarAlteracoesAsync();
    }

    public async Task AtivarAsync(int id)
    {
        var unidade = await _unidadeRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Unidade franqueada não encontrada.");

        unidade.Situacao = SituacaoUnidade.Ativa;
        await _unidadeRepository.SalvarAlteracoesAsync();
    }

    private static void AtribuirResponsaveis(UnidadeFranqueada unidade, List<FranqueadoDto> responsaveis)
    {
        foreach (var responsavelDto in responsaveis)
        {
            unidade.Responsaveis.Add(new Franqueado
            {
                Nome = responsavelDto.Nome,
                Cpf = responsavelDto.Cpf,
                Email = responsavelDto.Email,
                Telefone = responsavelDto.Telefone,
                Cargo = responsavelDto.Cargo,
                ResponsavelPrincipal = responsavelDto.ResponsavelPrincipal
            });
        }
    }

    private static UnidadeFranqueadaRespostaDto MapearParaDto(UnidadeFranqueada unidade)
    {
        return new UnidadeFranqueadaRespostaDto
        {
            Id = unidade.Id,
            FranqueadoraId = unidade.FranqueadoraId,
            Nome = unidade.Nome,
            Cnpj = unidade.Cnpj,
            Telefone = unidade.Telefone,
            Logradouro = unidade.Logradouro,
            Cidade = unidade.Cidade,
            Estado = unidade.Estado,
            Cep = unidade.Cep,
            DataInicio = unidade.DataInicio,
            Situacao = unidade.Situacao,
            PercentualRoyalty = unidade.PercentualRoyalty,
            Responsaveis = unidade.Responsaveis.Select(r => new FranqueadoDto
            {
                Id = r.Id,
                Nome = r.Nome,
                Cpf = r.Cpf,
                Email = r.Email,
                Telefone = r.Telefone,
                Cargo = r.Cargo,
                ResponsavelPrincipal = r.ResponsavelPrincipal
            }).ToList()
        };
    }
}
