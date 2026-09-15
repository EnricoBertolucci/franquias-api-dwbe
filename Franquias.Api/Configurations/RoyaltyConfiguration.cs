using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

public class RoyaltyConfiguration : IEntityTypeConfiguration<Royalty>
{
    public void Configure(EntityTypeBuilder<Royalty> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.FaturamentoBase).HasColumnType("decimal(18,2)");
        builder.Property(r => r.PercentualAplicado).HasColumnType("decimal(5,2)");
        builder.Property(r => r.ValorCalculado).HasColumnType("decimal(18,2)");
        builder.Property(r => r.StatusPagamento).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(r => new { r.UnidadeFranqueadaId, r.AnoReferencia, r.MesReferencia }).IsUnique();

        builder.HasOne(r => r.UnidadeFranqueada)
            .WithMany(u => u.Royalties)
            .HasForeignKey(r => r.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
