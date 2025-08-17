using Ordering.Contracts;
using Ordering.Core.Events;

namespace Ordering.Application.DomainEventHandlers;

internal class PublishEventOnOrderCreated
    : DomainEventHandler<OrderCreated>
{
    private readonly IEventBus _eventBus;

    public PublishEventOnOrderCreated(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(OrderCreated domainEvent, CancellationToken cancellationToken = default)
    {
        var integrationEvent = new OrderCreatedIntegrationEvent(
            domainEvent.OrderId,
            domainEvent.CustomerId,
            domainEvent.TotalAmount);

        await _eventBus.PublishAsync(integrationEvent);
    }
}
