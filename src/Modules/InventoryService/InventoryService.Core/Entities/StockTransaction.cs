namespace InventoryService.Core.Entities;

public class StockTransaction : Entity<Guid>
{
    public Guid StockItemId { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public int Quantity { get; private set; }
    public string? Reason { get; private set; }
    public DateTime Timestamp { get; set; }
    public string MessageId { get; private set; }
}
