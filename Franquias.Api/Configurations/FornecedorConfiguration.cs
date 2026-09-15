using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.RazaoSocial).IsRequired().HasMaxLength(150);
        builder.Property(f => f.Cnpj).IsRequired().HasMaxLength(18);
        builder.Property(f => f.Email).HasMaxLength(150);
        builder.Property(f => f.Telefone).HasMaxLength(20);

        builder.HasIndex(f => f.Cnpj).IsUnique();
    }
}
