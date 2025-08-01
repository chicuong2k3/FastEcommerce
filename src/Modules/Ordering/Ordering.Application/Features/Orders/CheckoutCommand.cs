namespace Ordering.Application.Orders;

public record CheckoutCommand(
    Guid CustomerId,
    string? Street,
    string Ward,
    string District,
    string Province,
    string Country,
    string PhoneNumber,
    string PaymentMethod,
    string ShippingMethod,
    decimal? Tax) : ICommand;

internal sealed class CheckoutCommandHandler(
    IOrderRepository orderRepository,
    ICartRepository cartRepository)
    : ICommandHandler<CheckoutCommand>
{
    public async Task<Result> Handle(CheckoutCommand command, CancellationToken cancellationToken)
    {
        var location = new Location(
               command.Street,
               command.Ward,
               command.District,
               command.Province,
               command.Country);

        if (!Enum.TryParse<ShippingMethod>(command.ShippingMethod, out var shippingMethod))
        {
            var shippingMethods = string.Join(",", Enum.GetNames(typeof(ShippingMethod)));
            return Result.Fail(new Error($"Invalid shipping method. Valid shipping methods: {shippingMethods}"));
        }

        var shippingInfo = new ShippingInfo(location, command.PhoneNumber, shippingMethod);

        if (!Enum.TryParse<PaymentMethod>(command.PaymentMethod, out var parsedPaymentMethod))
        {
            var paymentMethods = string.Join(",", Enum.GetNames(typeof(PaymentMethod)));
            return Result.Fail(new Error($"Invalid payment method. Valid payment methods: {paymentMethods}"));
        }
        var paymentInfo = new PaymentInfo(parsedPaymentMethod);


        var cart = await cartRepository.GetAsync(command.CustomerId, cancellationToken);

        if (cart == null)
            return Result.Fail(new Error("There is no items in cart"));

        var cartItems = cart.Items.ToList();

        if (!cartItems.Any())
            return Result.Fail(new Error("There is no items in cart"));

        var orderItems = new List<OrderItem>();
        foreach (var item in cartItems)
        {
            //if (!product.IsSimple && product.Variants.Any())
            //{
            //    var variant = product.Variants.FirstOrDefault(v => v.Id == item.ProductVariantId);
            //    if (variant == null)
            //        return Result.Fail(new Error($"Product variant with ID {item.ProductVariantId} not found for product {item.ProductId}."));

            //    basePriceAmount = variant.BasePrice;
            //    salePriceAmount = variant.SalePrice ?? basePriceAmount;

            //    if (!IsOnSale(variant.SaleFrom, variant.SaleTo))
            //    {
            //        salePriceAmount = null;
            //    }
            //}

            //var basePrice = Money.FromDecimal(basePriceAmount);
            //var salePrice = Money.FromDecimal(salePriceAmount);

            //if (item.BasePrice != basePrice || item.SalePrice != salePrice)
            //{
            //    return Result.Fail(new Error($"Price for product {product.Name} has changed. Please review your cart."));
            //}

            var orderItem = new OrderItem(
                item.ProductId,
                item.ProductVariantId,
                item.ProductName,
                item.Quantity,
                item.BasePrice,
                item.SalePrice,
                item.ImageUrl,
                item.VariantAttributes);

            orderItems.Add(orderItem);
        }


        var orderCreationResult = Order.Create(
            command.CustomerId,
            paymentInfo,
            shippingInfo,
            Money.FromDecimal(command.Tax),
            orderItems);

        if (orderCreationResult.IsFailed)
            return Result.Fail(orderCreationResult.Errors);

        var order = orderCreationResult.Value;

        await orderRepository.AddAsync(order, cancellationToken);
        return Result.Ok();
    }

    private bool IsOnSale(DateTime? saleFrom, DateTime? saleTo)
    {
        return (saleFrom == null || DateTime.UtcNow >= saleFrom) &&
               (saleTo == null || DateTime.UtcNow <= saleTo);
    }
}
