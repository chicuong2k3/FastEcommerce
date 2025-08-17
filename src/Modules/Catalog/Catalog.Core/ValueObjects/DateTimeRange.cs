namespace Catalog.Core.ValueObjects;

public record DateTimeRange : ValueObject
{
    public DateTime? From { get; }
    public DateTime? To { get; }

    public DateTimeRange(DateTime? from, DateTime? to)
    {
        From = from?.ToUniversalTime();
        To = to?.ToUniversalTime();
    }

    public override Result Validate()
    {
        if (From == null && To == null)
        {
            return Result.Fail("Either From or To date must be provided");
        }

        if (From != null && To != null && From >= To)
        {
            return Result.Fail("From date must be prior to To date");
        }

        return Result.Ok();
    }
}
