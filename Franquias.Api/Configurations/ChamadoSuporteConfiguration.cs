using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

public class ChamadoSuporteConfiguration : IEntityTypeConfiguration<ChamadoSuporte>
{
    public void Configure(EntityTypeBuilder<ChamadoSuporte> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Categoria).IsRequired().HasMaxLength(80);
        builder.Property(c => c.Descricao).IsRequired().HasMaxLength(1000);
        builder.Property(c => c.Prioridade).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasOne(c => c.UnidadeFranqueada)
            .WithMany(u => u.Chamados)
            .HasForeignKey(c => c.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Usuario)
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
