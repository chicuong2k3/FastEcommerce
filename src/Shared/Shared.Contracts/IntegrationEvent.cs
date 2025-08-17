using MassTransit;
using System.Text.Json.Serialization;

namespace Shared.Contracts;

[ExcludeFromTopology]
public abstract class IntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }

    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }


    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime occurredOn)
    {
        Id = id;
        OccurredOn = occurredOn;
    }
}
