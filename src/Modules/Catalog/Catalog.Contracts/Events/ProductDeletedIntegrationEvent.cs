using Shared.Contracts;
using System.Text.Json.Serialization;

namespace Catalog.Contracts.Events;

public class ProductDeletedIntegrationEvent : IntegrationEvent
{
    public ProductDeletedIntegrationEvent(Guid productId)
    {
        ProductId = productId;
    }

    [JsonConstructor]
    public ProductDeletedIntegrationEvent(Guid productId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; set; }
}
