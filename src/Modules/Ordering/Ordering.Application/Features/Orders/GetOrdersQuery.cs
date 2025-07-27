using Dapper;
using System.Data;
using System.Text;

namespace Ordering.Application.Orders;

public record GetOrdersQuery(
    int PageNumber,
    int PageSize,
    Guid? CustomerId,
    string? OrderStatus,
    DateTime? StartOrderDate,
    DateTime? EndOrderDate,
    decimal? MinTotal,
    decimal? MaxTotal,
    string? PaymentMethod) : IQuery<PaginationResult<OrderReadModel>>;

//internal class GetOrdersQueryHandler(IDbConnection dbConnection)
//    : IQueryHandler<GetOrdersQuery, PaginationResult<OrderReadModel>>
//{
//    public async Task<Result<PaginationResult<OrderReadModel>>> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
//    {
//        var specification = new GetOrdersSpecification()
//        {
//            PageNumber = query.PageNumber,
//            PageSize = query.PageSize,
//            CustomerId = query.CustomerId,
//            OrderStatus = Enum.TryParse(query.OrderStatus, out OrderStatus orderStatus) ? orderStatus : null,
//            StartOrderDate = query.StartOrderDate,
//            EndOrderDate = query.EndOrderDate,
//            MinTotal = query.MinTotal,
//            MaxTotal = query.MaxTotal,
//            PaymentMethod = Enum.TryParse(query.PaymentMethod, out PaymentMethod paymentMethod) ? paymentMethod : null
//        };

//        var queryBuilder = new StringBuilder("""
//            SELECT 
//                o."Id",
//                o."CustomerId",
//                o."Status",
//                o."OrderDate",
//                o."Total",
//                o."Subtotal",
//                o."PaymentMethod",
//                o."PhoneNumber",
//                o."ShippingMethod",
//                o."ShippingCosts",
//                o."Street",
//                o."Ward",
//                o."District",
//                o."Province",
//                o."Country"
//            FROM "ordering"."Orders" o
//            WHERE 1=1
//            """);

//        var parameters = new DynamicParameters();

//        if (specification.CustomerId.HasValue)
//        {
//            queryBuilder.Append(" AND o.\"CustomerId\" = @CustomerId");
//            parameters.Add("CustomerId", specification.CustomerId.Value);
//        }

//        if (specification.OrderStatus.HasValue)
//        {
//            queryBuilder.Append(" AND o.\"Status\" = @Status");
//            parameters.Add("Status", specification.OrderStatus.Value.ToString());
//        }

//        if (specification.StartOrderDate.HasValue)
//        {
//            queryBuilder.Append(" AND o.\"OrderDate\" >= @StartDate");
//            parameters.Add("StartDate", specification.StartOrderDate.Value);
//        }

//        if (specification.EndOrderDate.HasValue)
//        {
//            queryBuilder.Append(" AND o.\"OrderDate\" <= @EndDate");
//            parameters.Add("EndDate", specification.EndOrderDate.Value);
//        }

//        if (specification.MinTotal.HasValue)
//        {
//            queryBuilder.Append(" AND o.\"Total\" >= @MinTotal");
//            parameters.Add("MinTotal", specification.MinTotal.Value);
//        }

//        if (specification.MaxTotal.HasValue)
//        {
//            queryBuilder.Append(" AND o.\"Total\" <= @MaxTotal");
//            parameters.Add("MaxTotal", specification.MaxTotal.Value);
//        }

//        if (specification.PaymentMethod.HasValue)
//        {
//            queryBuilder.Append(" AND o.\"PaymentMethod\" = @PaymentMethod");
//            parameters.Add("PaymentMethod", specification.PaymentMethod.Value.ToString());
//        }

//        // Get total count
//        var countQuery = $"SELECT COUNT(*) FROM ({queryBuilder}) AS filtered";
//        var totalItems = await dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);

//        // Add pagination
//        queryBuilder.Append(" ORDER BY o.\"OrderDate\" DESC");
//        queryBuilder.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
//        parameters.Add("Offset", (specification.PageNumber - 1) * specification.PageSize);
//        parameters.Add("PageSize", specification.PageSize);

//        var orders = await dbConnection.QueryAsync<OrderReadModel>(queryBuilder.ToString(), parameters);
//        var ordersList = orders.ToList();

//        // Get items for all orders
//        if (ordersList.Any())
//        {
//            const string itemsQuery = """
//                SELECT 
//                    i."Id",
//                    i."OrderId",
//                    i."ProductId",
//                    i."ProductVariantId",
//                    i."OriginalPrice",
//                    i."SalePrice",
//                    i."Quantity",
//                    i."ProductName",
//                    i."Image",
//                    i."AttributesDescription"
//                FROM "ordering"."OrderItems" i
//                WHERE i."OrderId" = ANY(@OrderIds)
//                """;

//            var orderItems = await dbConnection.QueryAsync<OrderItemReadModel>(
//                itemsQuery,
//                new { OrderIds = ordersList.Select(o => o.Id).ToArray() });

//            // Group items by order
//            var itemsByOrder = orderItems.GroupBy(i => i.OrderId);
//            foreach (var group in itemsByOrder)
//            {
//                var order = ordersList.First(o => o.Id == group.Key);
//                order.Items = group.ToList();
//            }
//        }

//        return new PaginationResult<OrderReadModel>(
//            specification.PageNumber,
//            specification.PageSize,
//            totalItems,
//            ordersList);
//    }
//}
