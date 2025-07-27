using System.Text.Json.Serialization;

namespace Ordering.Core.Entities;

public class Cart : AggregateRoot<Guid>
{
    [JsonInclude]
    public Guid OwnerId { get; private set; }
    [JsonInclude]
    public List<CartItem> Items { get; private set; } = new();

    private Cart()
    {
    }

    public Cart(Guid ownerId)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        Items = new List<CartItem>();
    }

    public Result AddItem(
        Guid productId,
        Guid? productVariantId,
        string productName,
        string sku,
        int quantity,
        string? variantAttributes,
        string? imageUrl,
        Money basePrice,
        Money? salePrice)
    {
        var validationResult = basePrice.Validate();
        if (validationResult.IsFailed)
            return validationResult;
        if (salePrice != null)
        {
            validationResult = salePrice.Validate();
            return validationResult;
        }

        var cartItem = Items.FirstOrDefault(i => i.ProductId == productId && i.ProductVariantId == productVariantId);
        if (cartItem != null)
        {
            var increaseResult = cartItem.IncreaseQuantity(quantity);
            if (increaseResult.IsFailed)
                return increaseResult;
            Raise(new CartItemUpdated(Id, cartItem.Id));
            return Result.Ok();
        }

        var createResult = CartItem.Create(
            productId,
            productVariantId,
            productName,
            sku,
            quantity,
            variantAttributes,
            imageUrl,
            basePrice,
            salePrice);

        if (createResult.IsFailed)
            return Result.Fail(createResult.Errors);

        Items.Add(createResult.Value);
        Raise(new CartItemAdded(Id, createResult.Value.Id));

        return Result.Ok();
    }

    public Result RemoveItem(Guid productId, Guid? productVariantId, int quantity)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId && i.ProductVariantId == productVariantId);
        if (existingItem == null)
            return Result.Fail(new NotFoundError($"Cart item with product ID {productId} or product variant ID '{productVariantId}' not found."));

        var decreaseResult = existingItem.DecreaseQuantity(quantity);
        if (decreaseResult.IsFailed)
            return decreaseResult;

        if (existingItem.Quantity == 0)
        {
            Items.Remove(existingItem);
            Raise(new CartItemRemoved(Id, existingItem.Id));
        }
        else
        {
            Raise(new CartItemUpdated(Id, existingItem.Id));
        }

        return Result.Ok();
    }

    public Result UpdateItemDetails(
        Guid cartItemId,
        string productName,
        string? variantAttributes,
        string? imageUrl,
        Money basePrce,
        Money? salePrice)
    {
        var item = Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item == null)
            return Result.Fail(new NotFoundError($"Cart item with ID '{cartItemId}' not found."));

        var updateResult = item.UpdateDetails(productName, variantAttributes, imageUrl, basePrce, salePrice);
        if (updateResult.IsFailed)
            return updateResult;

        Raise(new CartItemUpdated(Id, item.Id));
        return Result.Ok();
    }
}
