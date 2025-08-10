namespace Catalog.Application.Mapping;

public static class ProductImageMapping
{
    public static ProductImageReadModel ToReadModel(this ProductImage productImage)
    {
        return new ProductImageReadModel
        {
            Id = productImage.Id,
            ProductId = productImage.ProductId,
            ProductAttributeId = productImage.ProductAttributeId,
            ProductAttributeValue = productImage.ProductAttributeValue,
            Url = productImage.Url,
            AltText = productImage.AltText,
            IsThumbnail = productImage.IsThumbnail,
            SortOrder = productImage.SortOrder
        };
    }
}
