using BlazorAutoBridge;
using Catalog.ReadModels;
using Catalog.Requests;
using RestEase;

namespace Administration.Apis;

[ApiService]
public interface IProductAttributeApi
{
    [Post("catalog-service/product-attributes")]
    Task<Response<ProductAttributeReadModel>> CreateProductAttributeAsync(
        [Body] CreateUpdateProductAttributeRequest request);

    [Delete("catalog-service/product-attributes/{id}")]
    Task<Response<object>> DeleteProductAttributeAsync(
        [Path] Guid id);

    [Get("catalog-service/product-attributes/{attributeId}/products/{productId}/values")]
    Task<Response<List<AttributeValueReadModel>>> GetAttributeValuesBelongToProductAsync(
        [Path] Guid attributeId,
        [Path] Guid productId);

    [Get("catalog-service/product-attributes/{id}")]
    Task<Response<ProductAttributeReadModel>> GetProductAttributeAsync(
        [Path] Guid id);

    [Get("catalog-service/product-attributes")]
    Task<Response<List<ProductAttributeReadModel>>> GetProductAttributesAsync();

    [Put("catalog-service/product-attributes/{id}")]
    Task<Response<ProductAttributeReadModel>> UpdateProductAttributeAsync(
        [Path] Guid id,
        [Body] CreateUpdateProductAttributeRequest request);



    [Get("catalog-service/product-attributes/{attributeId}/values")]
    Task<Response<List<AttributeValueReadModel>>> GetAttributeValuesAsync(
        [Path] Guid attributeId);

    [Post("catalog-service/product-attributes/{attributeId}/values")]
    Task<Response<List<AttributeValueReadModel>>> AddValueToAttributeAsync(
        [Path] Guid attributeId,
        [Body] AddValueForProductAttributeRequest request);

    [Post("catalog-service/product-attributes/{attributeId}/remove-values")]
    Task<Response<object>> RemoveValuesFromAttributeAsync(
        [Path] Guid attributeId,
        [Body] RemoveValuesFromAttributeRequest request);
}