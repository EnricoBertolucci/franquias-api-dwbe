using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

public class MovimentacaoEstoqueConfiguration : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Tipo).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Observacao).HasMaxLength(300);

        builder.HasOne(m => m.Estoque)
            .WithMany(e => e.Movimentacoes)
            .HasForeignKey(m => m.EstoqueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Usuario)
            .WithMany()
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Venda)
            .WithMany()
            .HasForeignKey(m => m.VendaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
