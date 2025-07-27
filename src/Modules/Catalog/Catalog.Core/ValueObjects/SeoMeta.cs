namespace Catalog.Core.ValueObjects;

public record SeoMeta
{
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string? Keywords { get; private set; }

    public SeoMeta(string? title, string? description, string? keywords)
    {
        Title = title;
        Description = description;
        Keywords = keywords;
    }
}
