using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

public class FranqueadoConfiguration : IEntityTypeConfiguration<Franqueado>
{
    public void Configure(EntityTypeBuilder<Franqueado> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nome).IsRequired().HasMaxLength(150);
        builder.Property(f => f.Cpf).IsRequired().HasMaxLength(14);
        builder.Property(f => f.Email).HasMaxLength(150);
        builder.Property(f => f.Telefone).HasMaxLength(20);
        builder.Property(f => f.Cargo).HasMaxLength(80);

        builder.HasOne(f => f.UnidadeFranqueada)
            .WithMany(u => u.Responsaveis)
            .HasForeignKey(f => f.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
