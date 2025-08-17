using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Persistence.Configurations;

internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.ProductName)
            .HasMaxLength(255);

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(1000);

        builder.OwnsOne(i => i.BasePrice, price =>
        {
            price.Property(p => p.Amount)
                .IsRequired()
                .HasColumnName("BasePriceAmount");
        });

        builder.OwnsOne(i => i.SalePrice, price =>
        {
            price.Property(p => p.Amount)
                .IsRequired()
                .HasColumnName("SalePriceAmount");
        });
    }
}
