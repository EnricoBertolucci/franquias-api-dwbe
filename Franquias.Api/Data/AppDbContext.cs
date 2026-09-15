using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Franqueadora> Franqueadoras => Set<Franqueadora>();
    public DbSet<UnidadeFranqueada> UnidadesFranqueadas => Set<UnidadeFranqueada>();
    public DbSet<Franqueado> Franqueados => Set<Franqueado>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<ProdutoServico> ProdutosServicos => Set<ProdutoServico>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<FornecedorProduto> FornecedoresProdutos => Set<FornecedorProduto>();
    public DbSet<Estoque> Estoques => Set<Estoque>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
    public DbSet<Royalty> Royalties => Set<Royalty>();
    public DbSet<ChamadoSuporte> ChamadosSuporte => Set<ChamadoSuporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
