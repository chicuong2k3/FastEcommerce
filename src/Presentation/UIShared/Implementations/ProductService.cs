using Catalog.ReadModels;
using Catalog.Requests;
using System.Net.Http.Json;
using System.Web;
using UIShared.Abstractions;
using UIShared.Models;

namespace UIShared.Implementations;

internal class ProductService : IProductService
{
    private const string BaseProductApi = "catalog-service/api/products";

    private readonly HttpClient _httpClient;
    private readonly ResponseHandler _responseHandler;

    public ProductService(IHttpClientFactory httpClientFactory, ResponseHandler responseHandler)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _responseHandler = responseHandler;
    }

    public async Task<Response<PaginationResult<ProductReadModel>>> GetProductsAsync(SearchProductsRequest request)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["pageNumber"] = request.PageNumber.ToString();
        query["pageSize"] = request.PageSize.ToString();

        if (request.CategoryId.HasValue)
            query["categoryId"] = request.CategoryId.ToString();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
            query["searchText"] = request.SearchText;

        if (!string.IsNullOrWhiteSpace(request.SortBy))
            query["sortBy"] = request.SortBy;

        if (request.MinPrice.HasValue)
            query["minPrice"] = request.MinPrice.ToString();

        if (request.MaxPrice.HasValue)
            query["maxPrice"] = request.MaxPrice.ToString();

        var response = await _httpClient.GetAsync($"{BaseProductApi}?{query}");
        return await _responseHandler.HandleResponse<PaginationResult<ProductReadModel>>(response);
    }

    public async Task<Response<ProductReadModel>> GetProductAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"{BaseProductApi}/{id}");
        return await _responseHandler.HandleResponse<ProductReadModel>(response);
    }

    public async Task<Response<ProductReadModel>> CreateProductAsync(CreateUpdateProductRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseProductApi, request);
        return await _responseHandler.HandleResponse<ProductReadModel>(response);
    }

    public async Task<Response<ProductReadModel>> UpdateProductAsync(Guid id, CreateUpdateProductRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseProductApi}/{id}", request);
        return await _responseHandler.HandleResponse<ProductReadModel>(response);
    }

    public async Task<Response> DeleteProductAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseProductApi}/{id}");
        return await _responseHandler.HandleResponse(response);
    }

    public async Task<Response> AddVariantAsync(Guid id, AddVariantRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseProductApi}/{id}/variants", request);
        return await _responseHandler.HandleResponse(response);
    }

    public async Task<Response<ProductVariantReadModel>> UpdateVariantAsync(Guid id, Guid variantId, UpdateVariantRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseProductApi}/{id}/variants/{variantId}", request);
        return await _responseHandler.HandleResponse<ProductVariantReadModel>(response);
    }

    public async Task<Response> RemoveVariantAsync(Guid productId, Guid variantId)
    {
        var response = await _httpClient.DeleteAsync($"{BaseProductApi}/{productId}/variants/{variantId}");
        return await _responseHandler.HandleResponse(response);
    }

    public async Task<Response<List<string>>> GetImagesAsync(Guid productId, GetImagesForProductRequest request)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        // if (request.AttributeValueId != null)
        //     query["attributeValueId"] = request.AttributeValueId.ToString();

        var response = await _httpClient.GetAsync($"{BaseProductApi}/{productId}/images?{query}");
        return await _responseHandler.HandleResponse<List<string>>(response);
    }

    public async Task<Response> AddImageAsync(Guid productId, AddImageForProductRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseProductApi}/{productId}/images", request);
        return await _responseHandler.HandleResponse(response);
    }

    public async Task<Response> RemoveImagesAsync(Guid productId, RemoveImagesFromProductRequest request)
    {
        var url = $"{BaseProductApi}/{productId}/images";
        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = JsonContent.Create(request)
        });
        return await _responseHandler.HandleResponse(response);
    }

    public async Task<Response<ThumbnailReadModel>> GetThumbnailAsync(Guid productId)
    {
        var response = await _httpClient.GetAsync($"{BaseProductApi}/{productId}/thumbnail");
        return await _responseHandler.HandleResponse<ThumbnailReadModel>(response);
    }
}
