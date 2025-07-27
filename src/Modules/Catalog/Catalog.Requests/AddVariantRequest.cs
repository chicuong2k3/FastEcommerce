namespace Catalog.Requests;

public class ProductAttributeValuePair
{
    public Guid ProductAttributeId { get; set; }
    public string ProductAttributeValue { get; set; }
}

public class AddVariantRequest
{
    public List<ProductAttributeValuePair> ProductAttributeValuePairs { get; set; } = [];
    public string? Sku { get; set; }

    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime? SaleFrom { get; set; }
    public DateTime? SaleTo { get; set; }
}