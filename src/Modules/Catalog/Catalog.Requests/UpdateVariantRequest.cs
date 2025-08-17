namespace Catalog.Requests;

public class UpdateVariantRequest
{
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime? SaleFrom { get; set; }
    public DateTime? SaleTo { get; set; }
}
