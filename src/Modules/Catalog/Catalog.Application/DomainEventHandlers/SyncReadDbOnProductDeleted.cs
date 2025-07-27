using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Catalog.Application.DomainEventHandlers;

internal class SyncReadDbOnProductDeleted : DomainEventHandler<ProductDeleted>
{
    private readonly ICatalogMongoContext _catalogMongoContext;

    public SyncReadDbOnProductDeleted(
        ICatalogMongoContext catalogMongoContext)
    {
        _catalogMongoContext = catalogMongoContext;
    }

    public override async Task Handle(ProductDeleted domainEvent, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ProductReadModel>.Filter.Eq(x => x.Id, domainEvent.ProductId);
        await _catalogMongoContext.Products.DeleteOneAsync(filter, cancellationToken: cancellationToken);
    }
}
