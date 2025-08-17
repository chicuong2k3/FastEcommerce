namespace InventoryService.Core.Events;

public record StockReleased(
    Guid StockId,
    Guid ProductId,
    Guid? VariantId,
    int Quantity) : DomainEvent;
