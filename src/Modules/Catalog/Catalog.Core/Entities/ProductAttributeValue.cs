namespace Catalog.Core.Entities;

public class ProductAttributeValue : Entity<Guid>
{
    public string Value { get; private set; }
    public Guid AttributeId { get; private set; }
    public ProductAttribute? Attribute { get; private set; }

    private ProductAttributeValue() { }

    public ProductAttributeValue(Guid attributeId, string value)
    {
        Id = Guid.NewGuid();
        Value = value;
        AttributeId = attributeId;
    }
}
