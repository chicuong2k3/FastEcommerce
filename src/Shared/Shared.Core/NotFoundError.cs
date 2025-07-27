using FluentResults;

namespace Shared.Core;

public class NotFoundError : Error
{
    public NotFoundError(string message) : base(message)
    {
    }
}