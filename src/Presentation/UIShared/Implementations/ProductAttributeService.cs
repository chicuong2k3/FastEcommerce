using Catalog.ReadModels;
using Catalog.Requests;
using System.Net.Http.Json;
using UIShared.Abstractions;
using UIShared.Models;

namespace UIShared.Implementations;

internal class ProductAttributeService : IProductAttributeService
{
    private readonly HttpClient _httpClient;
    private readonly ResponseHandler _responseHandler;

    private const string BaseProductAttributeRoute = "catalog-service/api/product-attributes";
    private const string ProductRoute = "catalog-service/api/products";

    public ProductAttributeService(IHttpClientFactory httpClientFactory, ResponseHandler responseHandler)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _responseHandler = responseHandler;
    }

    public async Task<Response<List<ProductAttributeReadModel>>> GetProductAttributesAsync()
    {
        var response = await _httpClient.GetAsync(BaseProductAttributeRoute);
        return await _responseHandler.HandleResponse<List<ProductAttributeReadModel>>(response);
    }

    public async Task<Response<ProductAttributeReadModel>> GetProductAttributeAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"{BaseProductAttributeRoute}/{id}");
        return await _responseHandler.HandleResponse<ProductAttributeReadModel>(response);
    }

    public async Task<Response<ProductAttributeReadModel>> CreateProductAttributeAsync(CreateUpdateProductAttributeRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseProductAttributeRoute, request);
        return await _responseHandler.HandleResponse<ProductAttributeReadModel>(response);
    }

    public async Task<Response<ProductAttributeReadModel>> UpdateProductAttributeAsync(Guid id, CreateUpdateProductAttributeRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseProductAttributeRoute}/{id}", request);
        return await _responseHandler.HandleResponse<ProductAttributeReadModel>(response);
    }

    public async Task<Response> DeleteProductAttributeAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseProductAttributeRoute}/{id}");
        return await _responseHandler.HandleResponse(response);
    }

    public async Task<Response<List<AttributeValueReadModel>>> GetAttributeValuesAsync(Guid attributeId)
    {
        var response = await _httpClient.GetAsync($"{BaseProductAttributeRoute}/{attributeId}/values");
        return await _responseHandler.HandleResponse<List<AttributeValueReadModel>>(response);
    }

    public async Task<Response> RemoveValuesFromAttributeAsync(Guid attributeId, RemoveValuesFromAttributeRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseProductAttributeRoute}/{attributeId}/values/remove-values", request);
        return await _responseHandler.HandleResponse(response);
    }

    public async Task<Response<List<AttributeValueReadModel>>> GetAttributeValuesBelongToProductAsync(Guid attributeId, Guid productId)
    {
        var response = await _httpClient.GetAsync($"{ProductRoute}/{productId}/attributes/{attributeId}/values");
        return await _responseHandler.HandleResponse<List<AttributeValueReadModel>>(response);
    }
}
