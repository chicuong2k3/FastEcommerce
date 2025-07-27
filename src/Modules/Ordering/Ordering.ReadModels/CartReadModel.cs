namespace Ordering.ReadModels;

public class CartReadModel
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public List<CartItemReadModel> Items { get; set; } = new();
}
