using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.HasKey(pva => pva.Id);

        builder.Property(pva => pva.Id)
            .ValueGeneratedNever();

        builder.Property(pva => pva.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(pva => pva.Attribute)
            .WithMany(a => a.Values)
            .HasForeignKey(pva => pva.AttributeId);
    }
}
