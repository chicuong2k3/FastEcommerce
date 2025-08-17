namespace Ordering.Application.Features.Carts;

public record RemoveItemFromCartCommand(
    Guid OwnerId,
    Guid ProductId,
    Guid? ProductVariantId,
    int Quantity) : ICommand;

internal sealed class RemoveItemFromCartCommandHandler(
    ICartRepository cartRepository)
    : ICommandHandler<RemoveItemFromCartCommand>
{
    public async Task<Result> Handle(RemoveItemFromCartCommand command, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(command.OwnerId, cancellationToken);
        if (cart == null)
        {
            return Result.Fail(new NotFoundError("Cart not found"));
        }

        var result = cart.RemoveItem(command.ProductId, command.ProductVariantId, command.Quantity);

        if (result.IsFailed)
        {
            return result;
        }

        await cartRepository.UpsertAsync(cart, cancellationToken);
        return result;
    }
}