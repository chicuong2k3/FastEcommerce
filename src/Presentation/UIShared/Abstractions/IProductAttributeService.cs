using Catalog.ReadModels;
using Catalog.Requests;
using UIShared.Models;

namespace UIShared.Abstractions
{
    public interface IProductAttributeService
    {
        Task<Response<ProductAttributeReadModel>> CreateProductAttributeAsync(CreateUpdateProductAttributeRequest request);
        Task<Response> DeleteProductAttributeAsync(Guid id);
        Task<Response<List<AttributeValueReadModel>>> GetAttributeValuesAsync(Guid attributeId);
        Task<Response<List<AttributeValueReadModel>>> GetAttributeValuesBelongToProductAsync(Guid attributeId, Guid productId);
        Task<Response<ProductAttributeReadModel>> GetProductAttributeAsync(Guid id);
        Task<Response<List<ProductAttributeReadModel>>> GetProductAttributesAsync();
        Task<Response> RemoveValuesFromAttributeAsync(Guid attributeId, RemoveValuesFromAttributeRequest request);
        Task<Response<ProductAttributeReadModel>> UpdateProductAttributeAsync(Guid id, CreateUpdateProductAttributeRequest request);
    }
}