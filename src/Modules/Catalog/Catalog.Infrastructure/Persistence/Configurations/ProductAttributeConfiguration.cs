using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pa => pa.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pa => pa.Unit)
            .HasMaxLength(20);
    }
}
