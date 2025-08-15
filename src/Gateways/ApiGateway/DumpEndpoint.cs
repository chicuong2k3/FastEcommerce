using FastEndpoints;

namespace ApiGateway;

public class DumpEndpoint : EndpointWithoutRequest<object>
{
    public override void Configure()
    {
        Get("/dump");
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken cancellationToken)
    {
        return SendOkAsync(new { Message = "This is a dump endpoint for debugging purposes." }, cancellationToken);
    }
}
