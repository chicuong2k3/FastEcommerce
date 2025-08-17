namespace Catalog.ReadModels;

public class ProductAttributeReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public bool IsOption { get; set; }
    public string? Unit { get; set; }
}