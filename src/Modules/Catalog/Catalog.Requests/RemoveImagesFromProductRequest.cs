namespace Catalog.Requests;

public class RemoveImagesFromProductRequest
{
    public List<string> ImageUrls { get; set; } = [];
}
