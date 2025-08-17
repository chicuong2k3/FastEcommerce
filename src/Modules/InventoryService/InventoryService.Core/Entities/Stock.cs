using InventoryService.Core.Events;

namespace InventoryService.Core.Entities;

public class Stock : AggregateRoot<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid? VariantId { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public bool IsActive { get; private set; }

    public Stock(Guid productId, Guid? variantId, int availableQuantity)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        VariantId = variantId;
        AvailableQuantity = availableQuantity;
        ReservedQuantity = 0;
        IsActive = true;
    }

    public void UpdateQuantity(int quantity)
    {
        AvailableQuantity += quantity;
    }

    public Result Reserve(int quantity)
    {
        if (AvailableQuantity < quantity)
            return Result.Fail($"Insufficient available quantity. Available Quantity: {AvailableQuantity}");

        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;

        Raise(new StockReserved(Id, ProductId, VariantId, quantity));

        return Result.Ok();
    }

    public Result Release(int quantity)
    {
        if (ReservedQuantity < quantity)
            return Result.Fail($"Insufficient reserved quantity. Reserved Quantity: {AvailableQuantity}");

        AvailableQuantity += quantity;
        ReservedQuantity -= quantity;

        Raise(new StockReleased(Id, ProductId, VariantId, quantity));

        return Result.Ok();
    }



    public Result Commit()
    {

        return Result.Ok();
    }
}
