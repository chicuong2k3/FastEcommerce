using Catalog.Application;
using Catalog.Core.Entities;
using Catalog.ReadModels;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Persistence;

internal class CatalogMongoContext : ICatalogMongoContext
{
    public IMongoCollection<ProductReadModel> Products { get; set; }

    public IMongoCollection<ProductImageReadModel> ProductImages { get; set; }

    public CatalogMongoContext(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("catalog");
        Products = database.GetCollection<ProductReadModel>("products");
        ProductImages = database.GetCollection<ProductImageReadModel>("product_images");
    }
}
