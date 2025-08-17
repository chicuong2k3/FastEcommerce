namespace Catalog.ReadModels;

public class AttributeValueReadModel
{
    public Guid AttributeId { get; set; }
    public string AttributeName { get; set; }
    public string AttributeValue { get; set; }
}