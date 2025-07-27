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

        builder.OwnsOne(p => p.Price, price =>
        {
            price.OwnsOne(x => x.BasePrice, basePrice =>
            {
                basePrice.Property(m => m.Amount)
                             .HasColumnName("BasePrice")
                             .IsRequired();
            });

            price.OwnsOne(x => x.SalePrice, salePrice =>
            {
                salePrice.Property(m => m.Amount)
                             .HasColumnName("SalePrice");
            });

            price.OwnsOne(x => x.SaleEffectiveRange, range =>
            {
                range.Property(r => r.From)
                         .HasColumnName("SaleFrom");
                range.Property(r => r.To)
                     .HasColumnName("SaleTo");
            });
        });

        builder.HasMany(pv => pv.AttributeValues)
            .WithMany();
    }
}
