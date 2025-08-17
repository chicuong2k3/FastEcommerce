namespace Catalog.ReadModels;

public class BrandReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string Slug { get; set; }
}
