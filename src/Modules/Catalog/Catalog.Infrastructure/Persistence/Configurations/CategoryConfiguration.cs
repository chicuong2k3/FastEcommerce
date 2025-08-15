using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.OwnsOne(c => c.SeoMeta, seoMeta =>
        {
            seoMeta.Property(m => m.Title)
                .HasMaxLength(100)
                .HasColumnName("MetaTitle");

            seoMeta.Property(m => m.Description)
                .HasMaxLength(500)
                .HasColumnName("MetaDescription");

            seoMeta.Property(m => m.Keywords)
                .HasMaxLength(100)
                .HasColumnName("MetaKeywords");
        });

        builder.HasMany(c => c.SubCategories)
            .WithOne()
            .HasForeignKey("ParentCategoryId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany<ProductCategory>()
            .WithOne()
            .HasForeignKey(pc => pc.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
