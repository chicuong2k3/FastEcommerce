namespace Ordering.Application.Mapping;

public static class CartItemMapping
{
    public static CartItemReadModel ToReadModel(this CartItem cartItem)
    {
        return new CartItemReadModel
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            ProductName = cartItem.ProductName,
            ProductVariantId = cartItem.ProductVariantId,
            Sku = cartItem.Sku,
            BasePrice = cartItem.BasePrice.Amount,
            SalePrice = cartItem.SalePrice?.Amount,
            Quantity = cartItem.Quantity,
            ImageUrl = cartItem.ImageUrl,
            VariantAttributes = cartItem.VariantAttributes,
            AddedAt = cartItem.AddedAt,
            UpdatedAt = cartItem.UpdatedAt
        };
    }
}
