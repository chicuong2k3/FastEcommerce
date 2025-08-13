using BlazorAutoBridge;
using RestEase;

namespace Administration.Apis;

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
