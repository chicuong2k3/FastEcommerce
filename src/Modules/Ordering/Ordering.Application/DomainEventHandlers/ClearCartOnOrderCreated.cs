using Ordering.Contracts;
using Ordering.Core.Events;

namespace Ordering.Application.DomainEventHandlers;

internal class ClearCartOnOrderCreated
    : DomainEventHandler<OrderCreated>
{
    private readonly ICartRepository _cartRepository;

    public ClearCartOnOrderCreated(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public override async Task Handle(OrderCreated domainEvent, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetAsync(domainEvent.CustomerId, cancellationToken);

        if (cart == null)
        {
            throw new ArgumentException($"Cart of customer with id '{domainEvent.CustomerId}' is null");
        }

        foreach (var orderItem in domainEvent.OrderItems)
        {
            cart.Items.RemoveAll(i => i.ProductId == orderItem.ProductId && i.ProductVariantId == orderItem.ProductVariantId);
        }

        await _cartRepository.UpsertAsync(cart);
    }
}
