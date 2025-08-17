namespace Catalog.Application.Mapping;

public static class BrandMapping
{
    public static BrandReadModel ToReadModel(this Brand brand)
    {
        return new BrandReadModel
        {
            Id = brand.Id,
            Name = brand.Name,
            Slug = brand.Slug,
            ImageUrl = brand.ImageUrl
        };
    }
}
