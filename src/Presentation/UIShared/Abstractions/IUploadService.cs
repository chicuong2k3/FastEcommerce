using UIShared.Models;

namespace UIShared.Abstractions
{
    public interface IUploadService
    {
        Task<Response> DeleteFileAsync(string fileName);
        Task<Response<string>> UploadFileAsync(byte[] fileBytes, string fileType);
    }
}