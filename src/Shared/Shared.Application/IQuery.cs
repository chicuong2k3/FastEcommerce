using FluentResults;
using MediatR;

namespace Shared.Application;

public interface IQuery<T> : IRequest<Result<T>>
{
}
