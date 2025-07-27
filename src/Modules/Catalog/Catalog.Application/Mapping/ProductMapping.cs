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
            CategoryIds = product.Categories.Select(c => c.Id).ToList(),
            Sku = product.Sku,
            IsSimple = product.IsSimple,
            MetaTitle = product.SeoMeta?.Title,
            MetaDescription = product.SeoMeta?.Description,
            MetaKeywords = product.SeoMeta?.Keywords,
            Variants = product.Variants.Select(v => v.ToReadModel()).ToList(),
            BasePrice = product.Price.BasePrice?.Amount,
            SalePrice = product.Price.SalePrice?.Amount,
            SaleFrom = product.Price.SaleEffectiveRange?.From,
            SaleTo = product.Price.SaleEffectiveRange?.To,
        };
    }

    public static ProductVariantReadModel ToReadModel(this ProductVariant variant)
    {
        return new ProductVariantReadModel
        {
            Id = variant.Id,
            Sku = variant.Sku,
            ProductAttributeValuePairs = variant.AttributeValues.Select(av => av.ToReadModel()).ToList(),
            BasePrice = variant.Price.BasePrice?.Amount,
            SalePrice = variant.Price.SalePrice?.Amount,
            SaleFrom = variant.Price.SaleEffectiveRange?.From,
            SaleTo = variant.Price.SaleEffectiveRange?.To,
        };
    }

    public static ProductImageReadModel ToReadModel(this ProductImage image)
    {
        if (image == null) throw new ArgumentNullException(nameof(image));
        return new ProductImageReadModel
        {
            Url = image.Url,
            AltText = image.AltText,
            IsThumbnail = image.IsThumbnail,
            SortOrder = image.SortOrder
        };
    }
}
