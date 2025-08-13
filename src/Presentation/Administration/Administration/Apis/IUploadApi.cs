using BlazorAutoBridge;
using RestEase;

namespace Administration.Apis;

[ApiService]
public interface IUploadApi
{
    [Post("media-service/upload")]
    Task<Response<string>> UploadFileAsync(
        [Header("accept")] string accept,
        [Header("Content-Type")] string contentType,
        [Body] MultipartFormDataContent fileContent
    );

    [Delete("media-service/upload/{fileName}")]
    Task<Response<object>> DeleteFileAsync(
        [Path] string fileName
    );
}
