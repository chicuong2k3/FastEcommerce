using FluentResults;

namespace Shared.Core;

public class ConflictError : Error
{
    public ConflictError(string message) : base(message)
    {
    }
}
