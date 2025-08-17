using InventoryService.Contracts.Events;
using InventoryService.Core.Events;

namespace InventoryService.Application.EventHandlers.DomainEvents;

internal class PublishEventOnStockReserved
    : DomainEventHandler<StockReserved>
{
    private readonly IEventBus _eventBus;

    public PublishEventOnStockReserved(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(StockReserved domainEvent, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(new StockReservedIntegrationEvent()
        {
            ProductId = domainEvent.ProductId
        });
    }
}
