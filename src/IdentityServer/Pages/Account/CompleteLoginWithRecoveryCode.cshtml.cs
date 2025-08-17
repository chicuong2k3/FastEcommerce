using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Account;

[IgnoreAntiforgeryToken]
public class CompleteLoginWithRecoveryCodeModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public CompleteLoginWithRecoveryCodeModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync(string recoveryCode, bool rememberMe, bool rememberMachine, string returnUrl)
    {
        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (!result.Succeeded)
        {
            return LocalRedirect($"/Account/LoginWith2fa?ReturnUrl={Uri.EscapeDataString(returnUrl ?? "/")}&errorMessage={Uri.EscapeDataString("Mã xác thực không hợp lệ.")}");
        }

        return LocalRedirect(returnUrl);
    }
}
