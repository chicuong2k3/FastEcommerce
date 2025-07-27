namespace Ordering.Core.ValueObjects;

public record PaymentInfo : ValueObject
{
    public PaymentMethod PaymentMethod { get; private set; }

    private PaymentInfo() { }

    public PaymentInfo(PaymentMethod paymentMethod)
    {
        PaymentMethod = paymentMethod;
    }

    public override Result Validate()
    {

        return Result.Ok();
    }
}
