using IdentityServer8.Services;
using IdentityServer8.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExternalLoginsController : ControllerBase
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IClientStore _clientStore;

    public ExternalLoginsController(
        IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IClientStore clientStore)
    {
        _interaction = interaction;
        _schemeProvider = schemeProvider;
        _clientStore = clientStore;
    }

    [HttpGet]
    public async Task<IActionResult> GetExternalLogins([FromQuery] string? returnUrl)
    {
        var schemes = await _schemeProvider.GetAllSchemesAsync();
        var externalLogins = schemes.Where(x => x.DisplayName != null).ToList();

        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

        if (context?.Client.ClientId != null)
        {
            var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
            if (client?.IdentityProviderRestrictions?.Any() == true)
            {
                externalLogins = externalLogins
                    .Where(p => client.IdentityProviderRestrictions.Contains(p.Name))
                    .ToList();
            }
        }

        return Ok(externalLogins.Select(p => new { p.Name, p.DisplayName }));
    }
}
