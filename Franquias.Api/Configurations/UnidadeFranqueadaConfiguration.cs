using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

public class UnidadeFranqueadaConfiguration : IEntityTypeConfiguration<UnidadeFranqueada>
{
    public void Configure(EntityTypeBuilder<UnidadeFranqueada> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Cnpj).IsRequired().HasMaxLength(18);
        builder.Property(u => u.Telefone).HasMaxLength(20);
        builder.Property(u => u.Logradouro).HasMaxLength(200);
        builder.Property(u => u.Cidade).HasMaxLength(100);
        builder.Property(u => u.Estado).HasMaxLength(2);
        builder.Property(u => u.Cep).HasMaxLength(9);
        builder.Property(u => u.PercentualRoyalty).HasColumnType("decimal(5,2)");

        builder.HasIndex(u => u.Cnpj).IsUnique();

        builder.HasOne(u => u.Franqueadora)
            .WithMany(f => f.Unidades)
            .HasForeignKey(u => u.FranqueadoraId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
