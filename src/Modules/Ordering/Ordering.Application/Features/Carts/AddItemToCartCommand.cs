using Catalog.Contracts.ApiClients;
using Catalog.ReadModels;
using InventoryService.Contracts.ApiClients;

namespace Ordering.Application.Features.Carts;

public record AddItemToCartCommand(Guid OwnerId,
    List<(Guid ProductId, Guid? ProductVariantId, int Quantity)> Items) : ICommand;

internal sealed class AddItemToCartHandler(
    ICartRepository cartRepository,
    IProductClient productClient,
    IStockClient stockClient)
    : ICommandHandler<AddItemToCartCommand>
{
    public async Task<Result> Handle(AddItemToCartCommand command, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(command.OwnerId, cancellationToken);

        if (cart == null)
        {
            cart = new Cart(command.OwnerId);
        }

        foreach (var item in command.Items)
        {
            var product = await productClient.GetProductByIdAsync(item.ProductId, cancellationToken);

            if (product == null)
            {
                return Result.Fail(new NotFoundError($"Product with id {item.ProductId} not found."));
            }

            ProductVariantReadModel? productVariant = null;
            if (!product.IsSimple)
            {
                productVariant = product.Variants.FirstOrDefault(v => v.Id == item.ProductVariantId);
                if (productVariant == null)
                {
                    return Result.Fail(new NotFoundError($"Product variant with id {item.ProductVariantId} not found."));
                }
            }
            else
            {

                if (item.ProductVariantId != null)
                {
                    return Result.Fail(new Error("Product variant ID should not be provided for simple products."));
                }
            }

            var stock = await stockClient.GetStockAsync(item.ProductId, item.ProductVariantId, cancellationToken);
            if (stock == null)
            {
                return Result.Fail("Stock unavailable");
            }

            if (stock.AvailableQuantity < item.Quantity)
            {
                return Result.Fail("Stock unavailable");
            }

            string imageUrl = string.Empty;
            var basePrice = product.IsSimple && product.BasePrice != null ? product.BasePrice.Value : productVariant?.BasePrice ?? 0m;

            var result = cart.AddItem(
                item.ProductId,
                item.ProductVariantId,
                product.Name,
                product.IsSimple && product.Sku != null ? product.Sku : productVariant?.Sku!,
                item.Quantity,
                product.IsSimple ? null : string.Join(",", productVariant?.ProductAttributeValuePairs.Select(x => $"{x.AttributeName}: {x.AttributeValue}") ?? []),
                imageUrl,
                Money.FromDecimal(basePrice),
                Money.FromDecimal(product.SalePrice));

            if (result.IsFailed)
                return result;
        }

        await cartRepository.UpsertAsync(cart, cancellationToken);

        return Result.Ok();
    }
}
