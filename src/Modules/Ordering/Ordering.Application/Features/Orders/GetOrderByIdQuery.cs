using Dapper;
using System.Data;

namespace Ordering.Application.Orders;

public record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderReadModel>;

internal sealed class GetOrderByIdQueryHandler(IDbConnection dbConnection)
    : IQueryHandler<GetOrderByIdQuery, OrderReadModel>
{
    public async Task<Result<OrderReadModel>> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        const string orderQuery = """
            SELECT 
                o."Id",
                o."CustomerId",
                o."Status",
                o."OrderDate",
                o."Total",
                o."Subtotal",
                o."PaymentMethod",
                o."PhoneNumber",
                o."ShippingMethod",
                o."ShippingCosts",
                o."Street",
                o."Ward",
                o."District",
                o."Province",
                o."Country"
            FROM "ordering"."Orders" o
            WHERE o."Id" = @Id
            """
        ;

        var order = await dbConnection.QueryFirstOrDefaultAsync<OrderReadModel>(orderQuery, new { Id = query.OrderId });

        if (order == null)
            return Result.Fail(new NotFoundError($"The order with id '{query.OrderId}' not found"));

        const string itemsQuery = """
            SELECT 
                i."Id",
                i."ProductId",
                i."ProductVariantId",
                i."OriginalPrice",
                i."SalePrice",
                i."Quantity",
                i."ProductName",
                i."Image",
                i."AttributesDescription"
            FROM "ordering"."OrderItems" i
            WHERE i."OrderId" = @OrderId
            """
        ;

        var items = await dbConnection.QueryAsync<OrderItemReadModel>(itemsQuery, new { query.OrderId });
        order.Items = items.ToList();


        return Result.Ok(order);
    }
}