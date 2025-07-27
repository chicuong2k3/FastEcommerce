
namespace Ordering.Core.ValueObjects;

public record ShippingInfo : ValueObject
{
    //public Money ShippingCosts { get; private set; }
    public Location ShippingAddress { get; private set; }
    public string PhoneNumber { get; private set; }
    public ShippingMethod ShippingMethod { get; private set; }

    private ShippingInfo() { }

    public ShippingInfo(
        Location shippingAddress,
        string phoneNumber,
        ShippingMethod shippingMethod)
    {
        ShippingAddress = shippingAddress;
        PhoneNumber = phoneNumber;
        ShippingMethod = shippingMethod;
    }

    public override Result Validate()
    {
        // var validationResult = ShippingCosts.Validate();
        // if (validationResult.IsFailed)
        //   return validationResult;

        //validationResult = ShippingAddress.Validate();
        //if (validationResult.IsFailed)
        //     return validationResult;

        return Result.Ok();
    }
}
