﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.ReadModels;

public class ProductReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? BrandId { get; set; }
    public string Slug { get; set; }
    [BsonRepresentation(BsonType.String)]
    public List<Guid> CategoryIds { get; set; } = [];
    public string? Sku { get; set; }
    public bool IsSimple { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public List<ProductVariantReadModel> Variants { get; set; } = new();

    public decimal? BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime? SaleFrom { get; set; }
    public DateTime? SaleTo { get; set; }
}