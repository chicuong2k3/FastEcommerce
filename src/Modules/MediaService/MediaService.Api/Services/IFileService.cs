namespace MediaService.Api.Services;

public interface IFileService
{
    Task<Uri> UploadAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string fileName, string folder, CancellationToken cancellationToken = default);
}
