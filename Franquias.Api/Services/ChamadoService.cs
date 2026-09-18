using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class ChamadoService : IChamadoService
{
    private readonly IChamadoRepository _chamadoRepository;
    private readonly IUnidadeFranqueadaRepository _unidadeRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ChamadoService(
        IChamadoRepository chamadoRepository,
        IUnidadeFranqueadaRepository unidadeRepository,
        IUsuarioRepository usuarioRepository)
    {
        _chamadoRepository = chamadoRepository;
        _unidadeRepository = unidadeRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ChamadoRespostaDto> CriarAsync(ChamadoCriacaoDto dto, int usuarioId)
    {
        var unidade = await _unidadeRepository.ObterPorIdAsync(dto.UnidadeFranqueadaId)
            ?? throw new KeyNotFoundException("Unidade franqueada não encontrada.");

        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var chamado = new ChamadoSuporte
        {
            UnidadeFranqueadaId = dto.UnidadeFranqueadaId,
            UnidadeFranqueada = unidade,
            UsuarioId = usuarioId,
            Usuario = usuario,
            Categoria = dto.Categoria,
            Prioridade = dto.Prioridade,
            Descricao = dto.Descricao,
            Status = StatusChamado.Aberto,
            DataAbertura = DateTime.UtcNow
        };

        await _chamadoRepository.AdicionarAsync(chamado);
        await _chamadoRepository.SalvarAlteracoesAsync();

        return MapearParaDto(chamado);
    }

    public async Task<ResultadoPaginadoDto<ChamadoRespostaDto>> ListarAsync(
        int? unidadeId,
        StatusChamado? status,
        PrioridadeChamado? prioridade,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        var (itens, total) = await _chamadoRepository.ListarAsync(
            unidadeId, status, prioridade, pagina, tamanhoPagina, ordenarPor, decrescente);

        return new ResultadoPaginadoDto<ChamadoRespostaDto>
        {
            Itens = itens.Select(MapearParaDto).ToList(),
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<ChamadoRespostaDto> ObterPorIdAsync(int id)
    {
        var chamado = await _chamadoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Chamado não encontrado.");

        return MapearParaDto(chamado);
    }

    public async Task<ChamadoRespostaDto> AtualizarAsync(int id, ChamadoAtualizacaoDto dto)
    {
        var chamado = await _chamadoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Chamado não encontrado.");

        if (chamado.Status == StatusChamado.Encerrado)
        {
            throw new InvalidOperationException("Não é possível atualizar um chamado já encerrado.");
        }

        if (dto.Status == StatusChamado.Encerrado)
        {
            throw new InvalidOperationException(
                "Use o endpoint de encerramento para fechar o chamado.");
        }

        if (dto.Status.HasValue)
        {
            chamado.Status = dto.Status.Value;
        }

        if (dto.Prioridade.HasValue)
        {
            chamado.Prioridade = dto.Prioridade.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Descricao))
        {
            chamado.Descricao = dto.Descricao;
        }

        chamado.DataAtualizacao = DateTime.UtcNow;

        await _chamadoRepository.SalvarAlteracoesAsync();

        return MapearParaDto(chamado);
    }

    public async Task<ChamadoRespostaDto> EncerrarAsync(int id)
    {
        var chamado = await _chamadoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Chamado não encontrado.");

        if (chamado.Status == StatusChamado.Encerrado)
        {
            throw new InvalidOperationException("O chamado já está encerrado.");
        }

        chamado.Status = StatusChamado.Encerrado;
        chamado.DataEncerramento = DateTime.UtcNow;
        chamado.DataAtualizacao = DateTime.UtcNow;

        await _chamadoRepository.SalvarAlteracoesAsync();

        return MapearParaDto(chamado);
    }

    private static ChamadoRespostaDto MapearParaDto(ChamadoSuporte chamado)
    {
        return new ChamadoRespostaDto
        {
            Id = chamado.Id,
            UnidadeFranqueadaId = chamado.UnidadeFranqueadaId,
            UnidadeFranqueadaNome = chamado.UnidadeFranqueada.Nome,
            UsuarioId = chamado.UsuarioId,
            UsuarioNome = chamado.Usuario.Nome,
            Categoria = chamado.Categoria,
            Prioridade = chamado.Prioridade,
            Descricao = chamado.Descricao,
            Status = chamado.Status,
            DataAbertura = chamado.DataAbertura,
            DataAtualizacao = chamado.DataAtualizacao,
            DataEncerramento = chamado.DataEncerramento
        };
    }
}
