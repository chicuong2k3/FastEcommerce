namespace Catalog.ReadModels;

public class ProductVariantReadModel
{
    public Guid Id { get; set; }
    public string? Sku { get; set; }
    public List<AttributeValueReadModel> ProductAttributeValuePairs { get; set; } = [];

    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime? SaleFrom { get; set; }
    public DateTime? SaleTo { get; set; }
}