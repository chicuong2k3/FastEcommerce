namespace Catalog.Requests;

public class CreateUpdateBrandRequest
{
    public string Name { get; set; }
    public string? Slug { get; set; }
    public string? ImageUrl { get; set; }
}