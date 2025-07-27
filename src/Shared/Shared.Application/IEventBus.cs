using Shared.Contracts;

namespace Shared.Application;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IntegrationEvent;
}
