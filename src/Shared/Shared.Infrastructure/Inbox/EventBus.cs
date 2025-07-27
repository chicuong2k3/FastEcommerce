using MassTransit;
using Shared.Application;
using Shared.Contracts;

namespace Shared.Infrastructure.Inbox;

internal class EventBus : IEventBus
{
    private readonly IBus _bus;

    public EventBus(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        await _bus.Publish(@event, cancellationToken);
    }
}
