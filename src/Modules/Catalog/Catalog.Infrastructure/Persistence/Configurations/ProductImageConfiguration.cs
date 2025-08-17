using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.Url)
            .HasMaxLength(1000);

        builder.Property(i => i.AltText)
            .HasMaxLength(250);

        builder.HasOne<ProductAttribute>()
            .WithMany()
            .HasForeignKey(i => i.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(i => i.ProductAttributeValue)
            .HasMaxLength(100);

    }
}
