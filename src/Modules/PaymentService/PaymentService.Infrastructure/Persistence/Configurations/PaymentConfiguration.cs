using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Core.Entities;
using PaymentService.Core.ValueObjects;

namespace PaymentService.Infrastructure.Persistence.Configurations;

internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.OwnsOne(x => x.TotalAmount, totalAmount =>
        {
            totalAmount.Property(x => x.Amount)
                       .HasColumnName("TotalAmount")
                       .IsRequired();
        });

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion(
                x => x.ToString(),
                x => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), x)
            );

        builder.Property(x => x.PaymentProvider)
            .IsRequired()
            .HasConversion(
                x => x.ToString(),
                x => (PaymentProvider)Enum.Parse(typeof(PaymentProvider), x)
            );

        builder.Property(x => x.PaymentUrl)
            .HasMaxLength(2048);

        builder.Property(x => x.PaymentToken)
            .HasMaxLength(512);

        builder.Property(x => x.ProviderTransactionId)
            .HasMaxLength(128);
    }
}
