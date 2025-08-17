using IdentityServer.Data;
using IdentityServer8.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Account;

[IgnoreAntiforgeryToken]
public class CompleteLoginWith2faModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public CompleteLoginWith2faModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync(string twoFactorCode, bool rememberMe, bool rememberMachine, string returnUrl)
    {
        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(twoFactorCode, rememberMe, rememberMachine);

        if (!result.Succeeded)
        {
            return LocalRedirect($"/Account/LoginWith2fa?ReturnUrl={Uri.EscapeDataString(returnUrl ?? "/")}&errorMessage={Uri.EscapeDataString("Mã xác thực không hợp lệ.")}");
        }

        return LocalRedirect(returnUrl);
    }
}
