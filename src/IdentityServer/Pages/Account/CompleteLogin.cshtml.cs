using IdentityServer.Data;
using IdentityServer8.Events;
using IdentityServer8.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Account;

[IgnoreAntiforgeryToken]
public class CompleteLoginModel : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public CompleteLoginModel(
        IIdentityServerInteractionService interaction,
        IEventService events,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _interaction = interaction;
        _events = events;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync(string email, string password, string returnUrl, bool rememberMe)
    {
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Unauthorized();

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: rememberMe, lockoutOnFailure: false);
        if (result.RequiresTwoFactor)
        {
            return LocalRedirect($"/Account/LoginWith2fa?ReturnUrl={Uri.EscapeDataString(returnUrl ?? "/")}&rememberMe={rememberMe}");
        }

        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

        return LocalRedirect(returnUrl);
    }
}
