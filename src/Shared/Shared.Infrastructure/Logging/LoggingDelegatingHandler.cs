using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Shared.Infrastructure.Logging;

public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingDelegatingHandler> _logger;

    public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
    {
        _logger = logger;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Sending {RequestMethod} request to {Url}", request.Method, request.RequestUri);
            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Received success response with status code '{StatusCode}' from {Url}", (int)response.StatusCode, response.RequestMessage?.RequestUri);
            }
            else
            {
                _logger.LogWarning("Received failure response with status code '{StatusCode}' from {Url}", (int)response.StatusCode, response.RequestMessage?.RequestUri);
            }

            return response;
        }
        catch (HttpRequestException ex)
            when (ex.InnerException is SocketException se && se.SocketErrorCode == SocketError.ConnectionRefused)
        {
            var hostWithPort = request.RequestUri!.IsDefaultPort
                ? request.RequestUri.DnsSafeHost
                : $"{request.RequestUri.DnsSafeHost}:{request.RequestUri.Port}";

            _logger.LogCritical(ex, "Failed to connect to {Host}", hostWithPort);
        }

        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            RequestMessage = request
        };
    }
}
