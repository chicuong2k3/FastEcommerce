using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace MediaService.Api.Services;

internal class FileService : IFileService
{
    private readonly Cloudinary _cloudinary;
    public FileService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<bool> DeleteAsync(string fileName, string folder, CancellationToken cancellationToken = default)
    {
        var deletionParams = new DeletionParams(Path.Combine(folder, fileName))
        {
            ResourceType = ResourceType.Image
        };

        var result = await _cloudinary.DestroyAsync(deletionParams);
        return result.Result == "ok";
    }

    public async Task<Uri> UploadAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Overwrite = true,
            Folder = folder
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl;
    }
}
