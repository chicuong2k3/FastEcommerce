using Shared.Core;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Shared.Infrastructure.Outbox;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; private set; }
    [Required]
    public string EventType { get; private set; }
    [Required]
    public string Content { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; set; }

    private OutboxMessage()
    {
    }

    public OutboxMessage(Guid id, string type, object content)
    {
        Id = id;
        EventType = type ?? throw new ArgumentNullException(nameof(type));
        Content = JsonSerializer.Serialize(content);
        OccurredOn = DateTime.UtcNow;
    }

    public DomainEvent DeserializeContent()
    {
        var eventType = DomainEventTypeRegistry.Resolve(EventType)
            ?? throw new InvalidOperationException($"Event type '{EventType}' could not be resolved.");

        if (!typeof(DomainEvent).IsAssignableFrom(eventType))
        {
            throw new InvalidOperationException($"Resolved type '{eventType.FullName}' does not inherit from DomainEvent.");
        }

        var deserializedEvent = JsonSerializer.Deserialize(Content, eventType)
            ?? throw new InvalidOperationException($"Failed to deserialize content for event type '{EventType}'.");

        return (DomainEvent)deserializedEvent;
    }
}
