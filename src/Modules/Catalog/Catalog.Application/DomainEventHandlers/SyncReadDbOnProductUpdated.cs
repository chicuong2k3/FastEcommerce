using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Catalog.Application.DomainEventHandlers;

internal class SyncReadDbOnProductUpdated : DomainEventHandler<ProductUpdated>
{
    private readonly ICatalogMongoContext _catalogMongoContext;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SyncReadDbOnProductUpdated> _logger;

    public SyncReadDbOnProductUpdated(
        ICatalogMongoContext catalogMongoContext,
        IProductRepository productRepository,
        ILogger<SyncReadDbOnProductUpdated> logger)
    {
        _catalogMongoContext = catalogMongoContext;
        _productRepository = productRepository;
        _logger = logger;
    }

    public override async Task Handle(ProductUpdated domainEvent, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(
            domainEvent.ProductId,
            cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found for syncing.", domainEvent.ProductId);
            return;
        }

        var productReadModel = product.ToReadModel();
        var filter = Builders<ProductReadModel>.Filter.Eq(x => x.Id, domainEvent.ProductId);
        await _catalogMongoContext.Products.ReplaceOneAsync(filter, productReadModel, cancellationToken: cancellationToken);
    }
}
