using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Catalog.Application.Features.Products;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductReadModel>;

internal sealed class GetProductByIdQueryHandler(
    IProductRepository productRepository,
    ILogger<GetProductByIdQueryHandler> logger)
    : IQueryHandler<GetProductByIdQuery, ProductReadModel>
{
    public async Task<Result<ProductReadModel>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(query.Id, cancellationToken);
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"Product with Id {query.Id} not found"));
        }

        var result = product.ToReadModel();

        logger.LogInformation("Product with Id {Id} retrieved successfully", query.Id);
        logger.LogInformation("Product details: {@Product}", JsonSerializer.Serialize(result));

        return Result.Ok(result);
    }
}