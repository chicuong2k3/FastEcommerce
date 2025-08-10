using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Catalog.Application.DomainEventHandlers;

internal class SyncReadDbOnProductImageAdded : DomainEventHandler<ProductImageAdded>
{
    private readonly ICatalogMongoContext _catalogMongoContext;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SyncReadDbOnProductVariantAdded> _logger;

    public SyncReadDbOnProductImageAdded(
        ICatalogMongoContext catalogMongoContext,
        IProductRepository productRepository,
        ILogger<SyncReadDbOnProductVariantAdded> logger)
    {
        _catalogMongoContext = catalogMongoContext;
        _productRepository = productRepository;
        _logger = logger;
    }

    public override async Task Handle(ProductImageAdded domainEvent, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(
            domainEvent.ProductId,
            cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found for syncing.", domainEvent.ProductId);
            return;
        }

        var productImage = product.Images.FirstOrDefault(x => x.Id == domainEvent.ProductImageId);
        if (productImage == null)
        {
            _logger.LogWarning("Product image with ID {ProductImageId} not found in product {ProductId}.",
                domainEvent.ProductImageId, domainEvent.ProductId);
            return;
        }

        _catalogMongoContext.ProductImages.InsertOne(
            productImage.ToReadModel());
    }
}
