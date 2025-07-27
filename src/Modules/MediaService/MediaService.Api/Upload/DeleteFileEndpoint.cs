using FastEndpoints;
using FluentResults;
using MediaService.Api.Services;
using Shared.Api;

namespace MediaService.Api.Upload;

public class DeleteFileEndpoint : EndpointWithoutRequest
{
    private readonly IFileService _fileService;

    public DeleteFileEndpoint(IFileService fileService)
    {
        _fileService = fileService;
    }

    public override void Configure()
    {
        Delete("/api/upload/{fileName}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var fileName = Route<string>("fileName") ?? string.Empty;

        var success = await _fileService.DeleteAsync(fileName, "ecommerce");
        if (success)
        {
            await SendNoContentAsync(ct);
        }
        else
        {
            await this.ToHttpResultAsync(Result.Fail($"Failed to delete file '{fileName}'."), ct);
        }
    }
}
