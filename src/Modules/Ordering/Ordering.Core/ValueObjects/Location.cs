
namespace Ordering.Core.ValueObjects;

public record Location : ValueObject
{
    public string? Street { get; private set; }
    public string Ward { get; private set; }
    public string District { get; private set; }
    public string Province { get; private set; }
    public string Country { get; private set; }

    private Location()
    {
    }

    public Location(string? street,
                    string ward,
                    string district,
                    string province,
                    string country)
    {
        Street = street;
        Ward = ward;
        District = district;
        Province = province;
        Country = country;
    }

    public override Result Validate()
    {
        return Result.Ok();
    }
}
