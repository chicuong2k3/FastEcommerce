using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Catalog.Application.DomainEventHandlers;

internal class SyncReadDbOnProductVariantAdded : DomainEventHandler<ProductVariantAdded>
{
    private readonly ICatalogMongoContext _catalogMongoContext;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SyncReadDbOnProductVariantAdded> _logger;

    public SyncReadDbOnProductVariantAdded(
        ICatalogMongoContext catalogMongoContext,
        IProductRepository productRepository,
        ILogger<SyncReadDbOnProductVariantAdded> logger)
    {
        _catalogMongoContext = catalogMongoContext;
        _productRepository = productRepository;
        _logger = logger;
    }

    public override async Task Handle(ProductVariantAdded domainEvent, CancellationToken cancellationToken = default)
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
        var filter = Builders<ProductReadModel>.Filter.Eq(p => p.Id, productReadModel.Id);
        var result = await _catalogMongoContext.Products.ReplaceOneAsync(
            filter,
            productReadModel,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);
    }
}
