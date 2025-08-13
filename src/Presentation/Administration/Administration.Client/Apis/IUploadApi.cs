using BlazorAutoBridge;
using RestEase;

namespace Administration.Client.Apis;

[ApiService]
public interface IUploadApi
{
    [Post("media-service/upload")]
    Task<Response<string>> UploadFileAsync(
        [Body] MultipartFormDataContent fileContent
    );

    [Delete("media-service/upload/{fileName}")]
    Task<Response<object>> DeleteFileAsync(
        [Path] string fileName
    );
}
