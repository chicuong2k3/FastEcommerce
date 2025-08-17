namespace Catalog.Core.Repositories;

public interface IProductAttributeRepository : IRepositoryBase<ProductAttribute, Guid>
{
    Task<ProductAttribute?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductAttribute?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<ProductAttribute>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<ProductAttribute>> GetAttributesAsync(CancellationToken cancellationToken = default);
    Task<List<ProductAttributeValue>> GetValuesAsync(Guid productAttributeId, CancellationToken cancellationToken = default);
}
