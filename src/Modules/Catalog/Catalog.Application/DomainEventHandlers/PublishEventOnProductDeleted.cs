using Catalog.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.DomainEventHandlers;

internal class PublishEventOnProductDeleted : DomainEventHandler<ProductDeleted>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<PublishEventOnProductDeleted> _logger;

    public PublishEventOnProductDeleted(IEventBus eventBus, ILogger<PublishEventOnProductDeleted> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public override async Task Handle(ProductDeleted domainEvent, CancellationToken cancellationToken = default)
    {
        var integrationEvent = new ProductDeletedIntegrationEvent(domainEvent.ProductId)
        {
            ProductId = domainEvent.ProductId,
        };

        await _eventBus.PublishAsync(integrationEvent);

        _logger.LogInformation("Published ProductDeleted integration event");
    }
}
