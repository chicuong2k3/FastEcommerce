namespace Shared.Core;

public interface IRepositoryBase<TEntity, TId>
    where TEntity : AggregateRoot<TId>
{
    Task<TEntity?> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
