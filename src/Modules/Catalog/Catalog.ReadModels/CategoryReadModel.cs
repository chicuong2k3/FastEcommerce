namespace Catalog.ReadModels;

public sealed class CategoryReadModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public IReadOnlyCollection<CategoryReadModel> SubCategories { get; set; } = new List<CategoryReadModel>();
    public Guid? ParentCategoryId { get; init; }

    public string Slug { get; set; }
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
    public string? PictureUrl { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}
