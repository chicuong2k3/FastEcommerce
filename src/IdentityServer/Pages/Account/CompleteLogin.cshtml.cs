using IdentityServer.Data;
using IdentityServer.Services;
using IdentityServer8;
using IdentityServer8.Events;
using IdentityServer8.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using static IdentityModel.OidcConstants;

namespace IdentityServer.Pages;

public class CompleteLoginModel : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILoginTokenStore _tokenStore;

    public CompleteLoginModel(
        IIdentityServerInteractionService interaction,
        IEventService events,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILoginTokenStore tokenStore)
    {
        _interaction = interaction;
        _events = events;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenStore = tokenStore;
    }


    public async Task<IActionResult> OnGetAsync(string token, string returnUrl, bool rememberMe)
    {
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

        var userId = await _tokenStore.ValidateTokenAsync(token);
        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        await _signInManager.SignInAsync(user, isPersistent: rememberMe);
        await _tokenStore.InvalidateTokenAsync(user.Id);

        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

        return LocalRedirect(returnUrl);
    }
}
