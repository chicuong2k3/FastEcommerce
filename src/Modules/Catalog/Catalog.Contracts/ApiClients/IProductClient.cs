using Catalog.ReadModels;

namespace Catalog.Contracts.ApiClients;

public interface IProductClient
{
    Task<ProductReadModel?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);
}
