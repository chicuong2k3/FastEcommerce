using Shared.Core;

namespace Ordering.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
