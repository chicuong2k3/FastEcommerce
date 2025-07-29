using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Account;


public class PerformExternalLoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public PerformExternalLoginModel(
        SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public IActionResult OnGet(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Page("/Account/ExternalLoginCallback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }
}
