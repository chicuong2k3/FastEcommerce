﻿using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
        .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(p => p.Sku)
            .HasMaxLength(100);

        builder.OwnsOne(p => p.SeoMeta, seoMeta =>
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

        builder.OwnsOne(p => p.Price, price =>
        {
            price.OwnsOne(x => x.BasePrice, basePrice =>
            {
                basePrice.Property(m => m.Amount)
                             .HasColumnName("BasePrice");
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

        builder.HasMany(p => p.Categories)
             .WithMany()
             .UsingEntity<Dictionary<Guid, Guid>>(
                 "ProductCategory",
                 j => j.HasOne<Category>()
                       .WithMany()
                       .HasForeignKey("CategoryId")
                       .OnDelete(DeleteBehavior.Cascade),
                 j => j.HasOne<Product>()
                       .WithMany()
                       .HasForeignKey("ProductId")
                       .OnDelete(DeleteBehavior.Cascade),
                 j =>
                 {
                     j.HasKey("ProductId", "CategoryId");
                     j.ToTable("ProductCategory");
                 });

        builder.HasMany(p => p.Variants)
                .WithOne()
                .HasForeignKey("ProductId").IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey("ProductId").IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.ProductAttributeValues)
            .WithMany();
    }
}
