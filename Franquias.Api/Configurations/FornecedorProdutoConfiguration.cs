using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

public class FornecedorProdutoConfiguration : IEntityTypeConfiguration<FornecedorProduto>
{
    public void Configure(EntityTypeBuilder<FornecedorProduto> builder)
    {
        builder.HasKey(fp => new { fp.FornecedorId, fp.ProdutoServicoId });

        builder.HasOne(fp => fp.Fornecedor)
            .WithMany(f => f.Produtos)
            .HasForeignKey(fp => fp.FornecedorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fp => fp.ProdutoServico)
            .WithMany(p => p.Fornecedores)
            .HasForeignKey(fp => fp.ProdutoServicoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
