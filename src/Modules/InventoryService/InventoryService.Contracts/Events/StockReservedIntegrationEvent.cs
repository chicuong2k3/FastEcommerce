using Shared.Contracts;

namespace InventoryService.Contracts.Events;

public class StockReservedIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
}
