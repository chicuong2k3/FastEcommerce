namespace Ordering.Application.Mapping;

public static class CartMapping
{
    public static CartReadModel ToReadModel(this Cart cart)
    {
        return new CartReadModel
        {
            Id = cart.Id,
            OwnerId = cart.OwnerId,
            Items = cart.Items.Select(i => i.ToReadModel()).ToList()
        };
    }
}
