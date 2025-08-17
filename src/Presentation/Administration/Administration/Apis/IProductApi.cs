using Administration.Client.Apis;
using BlazorAutoBridge;
using Catalog.ReadModels;
using Catalog.Requests;
using RestEase;

namespace Administration.Apis;

[ApiService]
public interface IProductApi
{
    [Post("catalog-service/products")]
    Task<Response<ProductReadModel>> CreateProductAsync([Body] CreateUpdateProductRequest request);

    [Put("catalog-service/products/{id}")]
    Task<Response<ProductReadModel>> UpdateProductAsync([Path] Guid id, [Body] CreateUpdateProductRequest request);

    [Delete("catalog-service/products/{id}")]
    Task<Response<object>> DeleteProductAsync([Path] Guid id);

    [Get("catalog-service/products/{id}")]
    Task<Response<ProductReadModel>> GetProductAsync([Path] Guid id);

    [Get("catalog-service/products")]
    Task<Response<PaginationResult<ProductReadModel>>> GetProductsAsync(
        [Query] int pageNumber,
        [Query] int pageSize,
        [Query] Guid? categoryId,
        [Query] string? searchText,
        [Query] string? sortBy,
        [Query] decimal? minPrice,
        [Query] decimal? maxPrice);

    [Get("catalog-service/products/{id}/thumbnail")]
    Task<Response<string>> GetThumbnailAsync([Path] Guid id);

    [Post("catalog-service/products/{id}/variants")]
    Task<Response<object>> AddVariantAsync([Path] Guid id, [Body] AddVariantRequest request);

    [Put("catalog-service/products/{id}/variants/{variantId}")]
    Task<Response<ProductVariantReadModel>> UpdateVariantAsync([Path] Guid id, [Path] Guid variantId, [Body] UpdateVariantRequest request);

    [Delete("catalog-service/products/{id}/variants/{variantId}")]
    Task<Response<object>> RemoveVariantAsync([Path] Guid id, [Path] Guid variantId);

    [Delete("catalog-service/products/{id}/images")]
    Task<Response<object>> RemoveImagesAsync([Path] Guid id, [Body] RemoveImagesFromProductRequest request);
}
