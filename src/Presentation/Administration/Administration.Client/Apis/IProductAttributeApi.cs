using BlazorAutoBridge;
using Catalog.ReadModels;
using Catalog.Requests;
using RestEase;

namespace Administration.Client.Apis;

[ApiService]
public interface IProductAttributeApi
{
    [Post("catalog-service/product-attributes")]
    Task<Response<ProductAttributeReadModel>> CreateProductAttributeAsync(
        [Body] CreateUpdateProductAttributeRequest request);

    [Delete("catalog-service/product-attributes/{id}")]
    Task<Response<object>> DeleteProductAttributeAsync(
        [Path] Guid id);

    [Get("catalog-service/product-attributes/{attributeId}/values")]
    Task<Response<List<AttributeValueReadModel>>> GetAttributeValuesAsync(
        [Path] Guid attributeId);

    [Get("catalog-service/product-attributes/{attributeId}/products/{productId}/values")]
    Task<Response<List<AttributeValueReadModel>>> GetAttributeValuesBelongToProductAsync(
        [Path] Guid attributeId,
        [Path] Guid productId);

    [Get("catalog-service/product-attributes/{id}")]
    Task<Response<ProductAttributeReadModel>> GetProductAttributeAsync(
        [Path] Guid id);

    [Get("catalog-service/product-attributes")]
    Task<Response<List<ProductAttributeReadModel>>> GetProductAttributesAsync();

    [Post("catalog-service/product-attributes/{attributeId}/remove-values")]
    Task<Response<object>> RemoveValuesFromAttributeAsync(
        [Path] Guid attributeId,
        [Body] RemoveValuesFromAttributeRequest request);

    [Put("catalog-service/product-attributes/{id}")]
    Task<Response<ProductAttributeReadModel>> UpdateProductAttributeAsync(
        [Path] Guid id,
        [Body] CreateUpdateProductAttributeRequest request);
}