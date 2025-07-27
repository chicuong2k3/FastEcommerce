
namespace InventoryService.Core.ValueObjects;

public record Address : ValueObject
{
    private Address() { }

    public Address(string country,
                   string city,
                   string province,
                   string? district,
                   string? addressLine1,
                   string? addressLine2)
    {
        Country = country;
        City = city;
        Province = province;
        District = district;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
    }

    public string Country { get; private set; }
    public string City { get; private set; }
    public string Province { get; private set; }
    public string? District { get; private set; }
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }

    public override Result Validate()
    {
        throw new NotImplementedException();
    }
}
