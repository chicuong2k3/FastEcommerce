using Ordering.Application.Mapping;

namespace Ordering.Application.Features.Carts;

public record GetCartQuery(Guid OwnerId) : IQuery<CartReadModel>;

internal sealed class GetCartQueryHandler(
    ICartRepository cartRepository)
    : IQueryHandler<GetCartQuery, CartReadModel>
{
    public async Task<Result<CartReadModel>> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(query.OwnerId, cancellationToken);
        if (cart == null)
        {
            return Result.Fail(new NotFoundError($"Cart not found for this user '{query.OwnerId}'"));
        }

        return Result.Ok(cart.ToReadModel());
    }
}