namespace InventoryService.Core.Events;

public record StockReserved(Guid StockId,
                            Guid ProductId,
                            Guid? VariantId,
                            int Quantity) : DomainEvent;
