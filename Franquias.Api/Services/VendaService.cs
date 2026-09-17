using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class VendaService : IVendaService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IUnidadeFranqueadaRepository _unidadeRepository;
    private readonly IProdutoServicoRepository _produtoRepository;
    private readonly IEstoqueRepository _estoqueRepository;

    public VendaService(
        IVendaRepository vendaRepository,
        IUnidadeFranqueadaRepository unidadeRepository,
        IProdutoServicoRepository produtoRepository,
        IEstoqueRepository estoqueRepository)
    {
        _vendaRepository = vendaRepository;
        _unidadeRepository = unidadeRepository;
        _produtoRepository = produtoRepository;
        _estoqueRepository = estoqueRepository;
    }

    public async Task<VendaRespostaDto> ObterPorIdAsync(int id)
    {
        var venda = await _vendaRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Venda não encontrada.");

        return MapearParaDto(venda);
    }

    public async Task<ResultadoPaginadoDto<VendaRespostaDto>> ListarAsync(
        int? unidadeId,
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        var (itens, total) = await _vendaRepository.ListarAsync(
            unidadeId, dataInicio, dataFim, pagina, tamanhoPagina, ordenarPor, decrescente);

        return new ResultadoPaginadoDto<VendaRespostaDto>
        {
            Itens = itens.Select(MapearParaDto).ToList(),
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<VendaRespostaDto> CriarAsync(VendaCriacaoDto dto, int usuarioId)
    {
        if (dto.Itens is null || dto.Itens.Count == 0)
        {
            throw new ArgumentException("A venda deve possuir ao menos um item.");
        }

        var unidade = await _unidadeRepository.ObterPorIdAsync(dto.UnidadeFranqueadaId)
            ?? throw new KeyNotFoundException("Unidade franqueada não encontrada.");

        if (unidade.Situacao != SituacaoUnidade.Ativa)
        {
            throw new InvalidOperationException("Unidades inativas não podem registrar vendas.");
        }

        var venda = new Venda
        {
            UnidadeFranqueadaId = dto.UnidadeFranqueadaId,
            UsuarioId = usuarioId,
            DataVenda = DateTime.UtcNow,
            Status = StatusVenda.Confirmada
        };

        var movimentacoes = new List<MovimentacaoEstoque>();
        decimal totalVenda = 0;

        foreach (var itemDto in dto.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(itemDto.ProdutoServicoId)
                ?? throw new KeyNotFoundException($"Produto/serviço {itemDto.ProdutoServicoId} não encontrado.");

            var estoque = await _estoqueRepository.ObterPorUnidadeEProdutoAsync(dto.UnidadeFranqueadaId, itemDto.ProdutoServicoId);

            if (estoque is null || estoque.QuantidadeAtual < itemDto.Quantidade)
            {
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto '{produto.Nome}' na unidade informada.");
            }

            var subtotal = produto.PrecoBase * itemDto.Quantidade;
            totalVenda += subtotal;

            venda.Itens.Add(new ItemVenda
            {
                ProdutoServicoId = produto.Id,
                ProdutoServico = produto,
                Quantidade = itemDto.Quantidade,
                PrecoUnitario = produto.PrecoBase,
                Subtotal = subtotal
            });

            estoque.QuantidadeAtual -= itemDto.Quantidade;

            movimentacoes.Add(new MovimentacaoEstoque
            {
                Estoque = estoque,
                Tipo = TipoMovimentacaoEstoque.Saida,
                Quantidade = itemDto.Quantidade,
                Observacao = "Baixa automática por venda.",
                DataMovimentacao = DateTime.UtcNow,
                UsuarioId = usuarioId,
                Venda = venda
            });
        }

        venda.ValorTotal = totalVenda;

        await _vendaRepository.RegistrarVendaAsync(venda, movimentacoes);

        return MapearParaDto(venda);
    }

    private static VendaRespostaDto MapearParaDto(Venda venda)
    {
        return new VendaRespostaDto
        {
            Id = venda.Id,
            UnidadeFranqueadaId = venda.UnidadeFranqueadaId,
            UsuarioId = venda.UsuarioId,
            DataVenda = venda.DataVenda,
            ValorTotal = venda.ValorTotal,
            Status = venda.Status,
            Itens = venda.Itens.Select(i => new ItemVendaRespostaDto
            {
                ProdutoServicoId = i.ProdutoServicoId,
                ProdutoNome = i.ProdutoServico.Nome,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}
