namespace IdentityServer.Api.Requests;

public class SetPasswordRequest
{
    public string NewPassword { get; set; } = default!;
}
