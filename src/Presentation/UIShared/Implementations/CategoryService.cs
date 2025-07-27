using Catalog.ReadModels;
using Catalog.Requests;
using System.Net.Http.Json;
using System.Web;
using UIShared.Abstractions;
using UIShared.Models;

namespace UIShared.Implementations;

internal class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;
    private readonly ResponseHandler _responseHandler;

    private const string BaseCategoryApi = "catalog-service/api/categories";

    public CategoryService(IHttpClientFactory httpClientFactory, ResponseHandler responseHandler)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _responseHandler = responseHandler;
    }

    public async Task<Response<List<CategoryReadModel>>> GetCategoriesAsync(GetCategoriesRequest request)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        if (request.Level.HasValue)
            query["level"] = request.Level.ToString();

        var response = await _httpClient.GetAsync($"{BaseCategoryApi}?{query}");
        return await _responseHandler.HandleResponse<List<CategoryReadModel>>(response);
    }

    public async Task<Response<CategoryReadModel>> GetCategoryAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"{BaseCategoryApi}/{id}");
        return await _responseHandler.HandleResponse<CategoryReadModel>(response);
    }

    public async Task<Response<CategoryReadModel>> CreateCategoryAsync(CreateUpdateCategoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseCategoryApi, request);
        return await _responseHandler.HandleResponse<CategoryReadModel>(response);
    }

    public async Task<Response<CategoryReadModel>> UpdateCategoryAsync(Guid id, CreateUpdateCategoryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseCategoryApi}/{id}", request);
        return await _responseHandler.HandleResponse<CategoryReadModel>(response);
    }

    public async Task<Response> DeleteCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseCategoryApi}/{id}");
        return await _responseHandler.HandleResponse(response);
    }
}
