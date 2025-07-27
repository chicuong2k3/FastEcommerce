namespace Catalog.Core.Repositories;

public interface IBrandRepository : IRepositoryBase<Brand, Guid>
{
    Task<IReadOnlyCollection<Brand>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Brand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
