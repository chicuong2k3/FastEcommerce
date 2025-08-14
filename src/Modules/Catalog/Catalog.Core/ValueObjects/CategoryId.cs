
namespace Catalog.Core.ValueObjects;

public record CategoryId : ValueObject
{
    public Guid Value { get; }

    public CategoryId(Guid value)
    {
        Value = value;
    }

    public override Result Validate()
    {
        return Result.Ok();
    }
}
