namespace Catalog.Core.Repositories;

public interface IProductRepository : IRepositoryBase<Product, Guid>
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
}
