using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Persistence.Configurations;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedNever();

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

        builder.Property(t => t.ProductId)
            .IsRequired();

        builder.Property(t => t.ProductVariantId)
            .IsRequired();

        builder.Property(t => t.Quantity)
            .IsRequired();


        builder.Property(t => t.ProductName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.ImageUrl)
            .HasMaxLength(2000);

        builder.Property(t => t.AttributesDescription)
            .HasMaxLength(2000);
    }
}
