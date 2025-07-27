
namespace Catalog.Core.Entities;

public sealed class Category : AggregateRoot<Guid>
{
    private Category()
    {

    }

    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public bool IsPublished { get; private set; }
    public string? PictureUrl { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    private List<Category> _subCategories = [];

    public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();

    public SeoMeta? SeoMeta { get; set; }

    public Category(string name,
                    string? description,
                    string? slug,
                    string? pictureUrl,
                    SeoMeta? seoMeta)
    {
        Id = Guid.NewGuid();
        Name = name.ToLower();
        Description = description;
        IsPublished = true;
        Slug = string.IsNullOrEmpty(slug) ? SlugHelper.GenerateSlug(Name) : slug;
        PictureUrl = pictureUrl;
        SeoMeta = seoMeta;
    }

    public void Update(
        string name,
        string? description,
        string? slug,
        string? pictureUrl,
        SeoMeta? seoMeta)
    {
        if (Name != name)
        {
            Name = name.ToLower();
        }

        if (!string.IsNullOrEmpty(description))
        {
            Description = description;
        }

        if (!string.IsNullOrEmpty(slug))
        {
            Slug = slug;
        }

        if (!string.IsNullOrEmpty(pictureUrl))
        {
            PictureUrl = pictureUrl;
        }

        SeoMeta = seoMeta;
    }

    public async Task<Result> AddSubCategoryAsync(Category subCategory, ICategoryRepository categoryRepository)
    {
        var isCircular = await IsCircularAsync(subCategory.Id, categoryRepository);
        if (isCircular)
            return Result.Fail(new ConflictError("Circular reference detected."));

        _subCategories.Add(subCategory);
        return Result.Ok();
    }

    private async Task<bool> IsCircularAsync(Guid childId, ICategoryRepository categoryRepository)
    {
        Category? current = this;
        while (current != null && current.ParentCategoryId != null)
        {
            if (current.ParentCategoryId == childId)
                return true;

            current = await categoryRepository.GetByIdAsync(current.ParentCategoryId.Value);
        }
        return false;
    }

}
