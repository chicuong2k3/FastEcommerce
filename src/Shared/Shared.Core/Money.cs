using FluentResults;
using System.Text.Json.Serialization;

namespace Shared.Core;

public record Money : ValueObject
{
    [JsonInclude]
    public decimal Amount { get; private set; }


    public Money() { }

    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Money FromDecimal(decimal amount)
    {
        return new Money(amount);
    }

    public static Money? FromDecimal(decimal? amount)
    {
        if (amount == null)
            return null;

        return new Money(amount.Value);
    }

    public override Result Validate()
    {
        if (Amount < 0)
        {
            return Result.Fail("Money amount cannot be negative");
        }

        return Result.Ok();
    }

    public static Money operator +(Money left, Money right)
    {
        return left with { Amount = left.Amount + right.Amount };
    }

    public static Money operator -(Money left, Money right)
    {
        return left with { Amount = left.Amount - right.Amount };
    }

    public static Money operator *(Money left, decimal right)
    {
        return left with { Amount = left.Amount * right };
    }

    public static implicit operator decimal(Money money) => money.Amount;

    public static bool operator >(Money left, Money right)
    {
        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        return left.Amount < right.Amount;
    }
}