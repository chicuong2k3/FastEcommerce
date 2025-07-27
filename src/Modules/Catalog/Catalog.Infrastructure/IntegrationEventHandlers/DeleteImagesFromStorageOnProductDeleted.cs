//using Shared.Infrastructure.Inbox;

//namespace Catalog.Infrastructure.IntegrationEventHandlers;

//public class DeleteImagesFromStorageOnProductDeleted
//    : IntegrationEventHandler<ProductDeletedIntegrationEvent>
//{
//    private readonly IFileService _fileService;

//    public DeleteImagesFromStorageOnProductDeleted(
//        IFileService fileService)
//    {
//        _fileService = fileService;
//    }

//    public override async Task Handle(ProductDeleted domainEvent, CancellationToken cancellationToken = default)
//    {
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
