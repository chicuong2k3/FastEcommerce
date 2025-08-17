namespace Catalog.Application.Mapping;

public static class ProductMapping
{
    public static ProductReadModel ToReadModel(this Product product)
    {
        return new ProductReadModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            BrandId = product.BrandId,
            Slug = product.Slug,
            CategoryIds = product.ProductCategories.Select(pc => pc.CategoryId).ToList(),
            Sku = product.Sku,
            IsSimple = product.IsSimple,
            MetaTitle = product.SeoMeta?.Title,
            MetaDescription = product.SeoMeta?.Description,
            MetaKeywords = product.SeoMeta?.Keywords,
            Variants = product.Variants.Select(v => v.ToReadModel()).ToList(),
            BasePrice = product.BasePrice?.Amount,
            SalePrice = product.SalePrice?.Amount,
            SaleFrom = product.SaleEffectiveRange?.From,
            SaleTo = product.SaleEffectiveRange?.To,
            ProductAttributeValuePairs = product.ProductAttributeValues.Select(av => av.ToReadModel()).ToList()
        };
    }

    public static ProductVariantReadModel ToReadModel(this ProductVariant variant)
    {
        return new ProductVariantReadModel
        {
            Id = variant.Id,
            Sku = variant.Sku,
            ProductAttributeValuePairs = variant.AttributeValues.Select(av => av.ToReadModel()).ToList(),
            BasePrice = variant.BasePrice.Amount,
            SalePrice = variant.SalePrice?.Amount,
            SaleFrom = variant.SaleEffectiveRange?.From,
            SaleTo = variant.SaleEffectiveRange?.To,
        };
    }
}
