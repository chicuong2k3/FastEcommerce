using MongoDB.Driver;

namespace Catalog.Application;

public interface ICatalogMongoContext
{
    IMongoCollection<ProductReadModel> Products { get; }
}
