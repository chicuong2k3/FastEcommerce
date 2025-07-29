using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityServer.Pages.Account;

public class ExternalLoginCallbackModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ExternalLoginCallbackModel> _logger;

    public ExternalLoginCallbackModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<ExternalLoginCallbackModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl = returnUrl ?? "/";

        if (remoteError != null)
        {
            _logger.LogInformation(remoteError);
            return LocalRedirect($"/Account/Login?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return LocalRedirect($"/Account/Login?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
        }

        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("{Name} logged in with {LoginProvider} provider.",
                info.Principal.Identity?.Name, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }

        var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Email claim not found in external login info.");
            return LocalRedirect($"/Account/Login?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
        }

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            var userLogins = await _userManager.GetLoginsAsync(existingUser);
            if (userLogins.All(x => x.LoginProvider != info.LoginProvider || x.ProviderKey != info.ProviderKey))
            {
                var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                if (!addLoginResult.Succeeded)
                {
                    _logger.LogError("Failed to add external login to existing user.");
                    return LocalRedirect($"/Account/Login?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
                }
            }

            await _signInManager.SignInAsync(existingUser, isPersistent: false);
            _logger.LogInformation("User {Email} signed in with new external provider {Provider}.", email, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }


        var newUser = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var createUserResult = await _userManager.CreateAsync(newUser);
        if (!createUserResult.Succeeded)
        {
            _logger.LogError("Failed to create new user with email {Email}.", email);
            return RedirectToPage("Login", new { ReturnUrl = returnUrl });
        }

        var addExternalLoginResult = await _userManager.AddLoginAsync(newUser, info);
        if (!addExternalLoginResult.Succeeded)
        {
            _logger.LogError("Failed to associate external login with new user.");
            return RedirectToPage("Login", new { ReturnUrl = returnUrl });
        }

        await _signInManager.SignInAsync(newUser, isPersistent: false);
        _logger.LogInformation("New user {Email} created and signed in with {Provider}.", email, info.LoginProvider);
        return LocalRedirect(returnUrl);
    }
}
