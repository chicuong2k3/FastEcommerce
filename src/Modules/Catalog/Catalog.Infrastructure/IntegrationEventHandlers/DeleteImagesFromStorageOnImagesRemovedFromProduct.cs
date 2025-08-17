//using Dapper;
//using Shared.Contracts;
//using System.Data;

//namespace Catalog.Infrastructure.IntegrationEventHandlers;

//public class DeleteImagesFromStorageOnImagesRemovedFromProduct
//    : IntegrationEvent<ImagesRemovedFromProduct>
//{
//    private readonly IFileService _fileService;
//    private readonly IDbConnection _dbConnection;

//    public DeleteImagesFromStorageOnImagesRemovedFromProduct(
//        IFileService fileService,
//        IDbConnection dbConnection)
//    {
//        _fileService = fileService;
//        _dbConnection = dbConnection;
//    }

//    public override async Task Handle(ImagesRemovedFromProduct domainEvent, CancellationToken cancellationToken = default)
//    {
//        const string deleteSql = """
//            DELETE FROM catalog."ProductImages" pi
//            WHERE pi."ImageUrl" = ANY(@ImageUrls)
//            """;

//        var rowsAffected = await _dbConnection.ExecuteAsync(
//            deleteSql,
//            new { ImageUrls = domainEvent.ImageUrls.ToArray() });

//        if (rowsAffected != domainEvent.ImageUrls.Count)
//        {
//            throw new InvalidOperationException("Failed to delete some or all ProductImages.");
//        }

//        foreach (var imageUrl in domainEvent.ImageUrls)
//        {
//            var fileName = Path.GetFileNameWithoutExtension(imageUrl);
//            var success = await _fileService.DeleteAsync(
//                fileName,
//                "ecommerce",
//                cancellationToken);

//            if (!success)
//            {
//                throw new InvalidOperationException($"Failed to delete image: {imageUrl}");
//            }
//        }
//    }
//}
