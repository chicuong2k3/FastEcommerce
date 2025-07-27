namespace MediaService.Api.Upload;

public record UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
}