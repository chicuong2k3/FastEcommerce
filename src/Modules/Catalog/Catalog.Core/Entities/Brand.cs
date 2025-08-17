namespace Catalog.Core.Entities;

public class Brand : AggregateRoot<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; }
    public string? ImageUrl { get; set; }

    public Brand(string name, string? slug = null, string? imageUrl = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Slug = string.IsNullOrEmpty(slug) ? SlugHelper.GenerateSlug(Name) : slug;
        ImageUrl = imageUrl;
    }

    private Brand() { }

    public void Update(string name, string? slug = null, string? imageUrl = null)
    {
        Name = name;
        Slug = string.IsNullOrEmpty(slug) ? SlugHelper.GenerateSlug(Name) : slug;
        if (!string.IsNullOrEmpty(imageUrl))
        {
            ImageUrl = imageUrl;
        }
    }
}
