namespace IdentityServer.Api.Responses;

public class GetAuthenticatorSetupInfoResponse
{
    public string SharedKey { get; set; } = string.Empty;
    public string AuthenticatorUri { get; set; } = string.Empty;
}
