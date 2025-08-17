using BlazorAutoBridge;
using Catalog.ReadModels;
using Catalog.Requests;
using RestEase;

namespace Administration.Client.Apis;

[ApiService]
public interface ICategoryApi
{
    [Post("catalog-service/categories")]
    Task<Response<CategoryReadModel>> CreateCategoryAsync([Body] CreateUpdateCategoryRequest request);

    [Delete("catalog-service/categories/{id}")]
    Task<Response<object>> DeleteCategoryAsync([Path] Guid id);

    [Get("catalog-service/categories")]
    Task<Response<List<CategoryReadModel>>> GetCategoriesAsync([Query] int? level);

    [Get("catalog-service/categories/{id}")]
    Task<Response<CategoryReadModel>> GetCategoryAsync([Path] Guid id);

    [Put("catalog-service/categories/{id}")]
    Task<Response<CategoryReadModel>> UpdateCategoryAsync([Path] Guid id, [Body] CreateUpdateCategoryRequest request);
}