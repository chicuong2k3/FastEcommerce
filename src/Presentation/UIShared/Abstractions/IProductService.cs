using Catalog.ReadModels;
using Catalog.Requests;
using UIShared.Models;

namespace UIShared.Abstractions;

public interface IProductService
{
    Task<Response> AddImageAsync(Guid productId, AddImageForProductRequest request);
    Task<Response> AddVariantAsync(Guid id, AddVariantRequest request);
    Task<Response<ProductReadModel>> CreateProductAsync(CreateUpdateProductRequest request);
    Task<Response> DeleteProductAsync(Guid id);
    Task<Response<List<string>>> GetImagesAsync(Guid productId, GetImagesForProductRequest request);
    Task<Response<ProductReadModel>> GetProductAsync(Guid id);
    Task<Response<PaginationResult<ProductReadModel>>> GetProductsAsync(SearchProductsRequest request);
    Task<Response<ThumbnailReadModel>> GetThumbnailAsync(Guid productId);
    Task<Response> RemoveImagesAsync(Guid productId, RemoveImagesFromProductRequest request);
    Task<Response> RemoveVariantAsync(Guid productId, Guid variantId);
    Task<Response<ProductReadModel>> UpdateProductAsync(Guid id, CreateUpdateProductRequest request);
    Task<Response<ProductVariantReadModel>> UpdateVariantAsync(Guid id, Guid variantId, UpdateVariantRequest request);
}