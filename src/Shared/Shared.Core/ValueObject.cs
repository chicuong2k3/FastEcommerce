using FluentResults;

namespace Shared.Core;

public abstract record ValueObject
{
    public abstract Result Validate();
}
