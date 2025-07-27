namespace Catalog.Application.Mapping;

public static class AttributeValueMapping
{
    public static AttributeValueReadModel ToReadModel(this ProductAttributeValue attributeValue)
    {
        return new AttributeValueReadModel
        {
            AttributeId = attributeValue.AttributeId,
            AttributeName = attributeValue.Attribute?.Name ?? string.Empty,
            AttributeValue = attributeValue.Value
        };
    }
}
