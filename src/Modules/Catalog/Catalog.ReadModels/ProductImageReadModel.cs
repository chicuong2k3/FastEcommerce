using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.ReadModels;

public class ProductImageReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = default!;
    public string Url { get; set; }
    public string? AltText { get; set; }
    public bool IsThumbnail { get; set; }
    public int SortOrder { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid? ProductAttributeId { get; set; }
    public string? ProductAttributeValue { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; }
}
