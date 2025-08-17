using System.Text.Json.Serialization;

namespace Shared.Core;

public abstract record DomainEvent
{
    public Guid Id { get; }

    public DateTime OccurredOn { get; }

    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }


    [JsonConstructor]
    public DomainEvent(Guid id, DateTime occurredOn)
    {
        Id = id;
        OccurredOn = occurredOn;
    }
}
