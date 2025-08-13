using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Sku)
            .HasMaxLength(100);

        builder.OwnsOne(p => p.BasePrice, basePrice =>
        {
            basePrice.Property(m => m.Amount)
                             .HasColumnName("BasePrice");
        });

        builder.OwnsOne(p => p.SalePrice, salePrice =>
        {
            salePrice.Property(m => m.Amount)
                             .HasColumnName("SalePrice");
        });

        builder.OwnsOne(p => p.SaleEffectiveRange, range =>
        {
            range.Property(r => r.From)
                         .HasColumnName("SaleFrom");
            range.Property(r => r.To)
                 .HasColumnName("SaleTo");
        });

        builder.HasMany(pv => pv.AttributeValues)
            .WithMany();
    }
}
