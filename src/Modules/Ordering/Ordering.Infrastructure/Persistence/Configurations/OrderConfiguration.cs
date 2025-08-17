using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Core.Entities;
using Ordering.Core.ValueObjects;

namespace Ordering.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.Status)
            .HasConversion(
                o => o.ToString(),
                o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
            );

        builder.OwnsOne(o => o.ShippingInfo, shipping =>
        {
            //shipping.OwnsOne(s => s.ShippingCosts, costs =>
            //{
            //    costs.Property(c => c.Amount)
            //        .IsRequired()
            //        .HasColumnName("ShippingCostsAmount");
            //});

            shipping.OwnsOne(s => s.ShippingAddress, addr =>
            {
                addr.Property(a => a.Street)
                    .HasMaxLength(100)
                    .HasColumnName("Street");

                addr.Property(a => a.Ward)
                    .HasMaxLength(50)
                    .HasColumnName("Ward");

                addr.Property(a => a.District)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("District");

                addr.Property(a => a.Province)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Province");

                addr.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Country");
            });

            shipping.Property(s => s.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("PhoneNumber");

            shipping.Property(s => s.ShippingMethod)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ShippingMethod")
                .HasConversion(
                    s => s.ToString(),
                    s => (ShippingMethod)Enum.Parse(typeof(ShippingMethod), s)
                );
        });

        builder.OwnsOne(o => o.PaymentInfo, payment =>
        {
            payment.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("PaymentMethod")
                .HasConversion(
                    p => p.ToString(),
                    p => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), p)
                );
        });

        builder.OwnsOne(o => o.Total, total =>
        {
            total.Property(t => t.Amount)
                .IsRequired()
                .HasColumnName("TotalAmount");
        });

        builder.OwnsOne(o => o.Subtotal, subtotal =>
        {
            subtotal.Property(t => t.Amount)
                .IsRequired()
                .HasColumnName("SubtotalAmount");
        });

        builder.OwnsOne(o => o.Tax, tax =>
        {
            tax.Property(t => t.Amount)
                .HasColumnName("TaxAmount");
        });

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
