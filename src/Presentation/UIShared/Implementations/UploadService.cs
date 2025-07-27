using System.Net.Http.Headers;
using UIShared.Abstractions;
using UIShared.Models;

namespace UIShared.Implementations;

internal class UploadService : IUploadService
{
    private readonly HttpClient _httpClient;
    private readonly ResponseHandler _responseHandler;
    public UploadService(IHttpClientFactory httpClientFactory, ResponseHandler responseHandler)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _responseHandler = responseHandler;
    }

    public async Task<Response<string>> UploadFileAsync(byte[] fileBytes, string fileType)
    {
        using var formData = new MultipartFormDataContent();
        var streamContent = new StreamContent(new MemoryStream(fileBytes));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileType);
        formData.Add(streamContent, "file", Guid.NewGuid().ToString());
        var response = await _httpClient.PostAsync("api/upload", formData);
        return await _responseHandler.HandleResponse<string>(response);
    }

    public async Task<Response> DeleteFileAsync(string fileName)
    {
        var response = await _httpClient.DeleteAsync($"/api/upload/{fileName}");
        return await _responseHandler.HandleResponse<string>(response);
    }
}