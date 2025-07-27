
using Catalog.ReadModels;
using Catalog.Requests;
using UIShared.Models;

namespace UIShared.Abstractions;

public interface ICategoryService
{
    Task<Response<CategoryReadModel>> CreateCategoryAsync(CreateUpdateCategoryRequest request);
    Task<Response> DeleteCategoryAsync(Guid id);
    Task<Response<List<CategoryReadModel>>> GetCategoriesAsync(GetCategoriesRequest request);
    Task<Response<CategoryReadModel>> GetCategoryAsync(Guid id);
    Task<Response<CategoryReadModel>> UpdateCategoryAsync(Guid id, CreateUpdateCategoryRequest request);
}
