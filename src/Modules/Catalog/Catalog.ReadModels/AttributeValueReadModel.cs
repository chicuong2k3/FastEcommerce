using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.ReadModels;

public class AttributeValueReadModel
{
    [BsonRepresentation(BsonType.String)]
    public Guid AttributeId { get; set; }
    public string AttributeName { get; set; }
    public string AttributeValue { get; set; }
}