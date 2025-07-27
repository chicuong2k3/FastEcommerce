using Microsoft.Extensions.Logging;

namespace Catalog.Application.DomainEventHandlers;

internal class SyncReadDbOnProductCreated : DomainEventHandler<ProductCreated>
{
    private readonly ICatalogMongoContext _catalogMongoContext;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SyncReadDbOnProductCreated> _logger;

    public SyncReadDbOnProductCreated(
        ICatalogMongoContext catalogMongoContext,
        IProductRepository productRepository,
        ILogger<SyncReadDbOnProductCreated> logger)
    {
        _catalogMongoContext = catalogMongoContext;
        _productRepository = productRepository;
        _logger = logger;
    }

    public override async Task Handle(ProductCreated domainEvent, CancellationToken cancellationToken = default)
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
        await _catalogMongoContext.Products.InsertOneAsync(productReadModel, cancellationToken: cancellationToken);
    }
}
