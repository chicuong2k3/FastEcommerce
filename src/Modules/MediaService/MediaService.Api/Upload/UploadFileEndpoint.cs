using FastEndpoints;
using FluentResults;
using MediaService.Api.Services;
using Shared.Api;

namespace MediaService.Api.Upload;

public class UploadFileEndpoint : Endpoint<UploadFileRequest, Uri>
{
    private readonly IFileService _fileService;

    public UploadFileEndpoint(IFileService fileService)
    {
        _fileService = fileService;
    }

    public override void Configure()
    {
        Post("/api/upload");
        AllowFormData();
        AllowAnonymous();
    }

    public override async Task HandleAsync(UploadFileRequest req, CancellationToken ct)
    {
        if (req.File == null)
        {
            await this.ToHttpResultAsync(Result.Fail("File is missing."), ct);
            return;
        }

        if (req.File.Length == 0)
        {
            await this.ToHttpResultAsync(Result.Fail("File is empty."), ct);
            return;
        }

        var fileUri = await _fileService.UploadAsync(req.File, "ecommerce");
        await SendAsync(fileUri, cancellation: ct);
    }
}

