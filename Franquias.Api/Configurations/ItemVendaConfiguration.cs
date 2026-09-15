using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.PrecoUnitario).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Subtotal).HasColumnType("decimal(18,2)");

        builder.HasOne(i => i.Venda)
            .WithMany(v => v.Itens)
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.ProdutoServico)
            .WithMany(p => p.ItensVenda)
            .HasForeignKey(i => i.ProdutoServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
