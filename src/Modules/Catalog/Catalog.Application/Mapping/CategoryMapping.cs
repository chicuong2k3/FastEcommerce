namespace Catalog.Application.Mapping;

public static class CategoryMapping
{
    public static CategoryReadModel ToReadModel(this Category category)
    {
        return new CategoryReadModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Slug = category.Slug,
            IsPublished = category.IsPublished,
            PictureUrl = category.PictureUrl,
            MetaTitle = category?.SeoMeta?.Title,
            MetaDescription = category?.SeoMeta?.Description,
            MetaKeywords = category?.SeoMeta?.Keywords
        };
    }
}
