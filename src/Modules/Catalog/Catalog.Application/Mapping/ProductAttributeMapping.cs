namespace Catalog.Application.Mapping;

public static class ProductAttributeMapping
{
    public static ProductAttributeReadModel ToReadModel(this ProductAttribute attribute)
    {
        return new ProductAttributeReadModel
        {
            Id = attribute.Id,
            Name = attribute.Name,
            DisplayName = attribute.DisplayName,
            IsOption = attribute.IsOption,
            Unit = attribute.Unit
        };
    }
}
