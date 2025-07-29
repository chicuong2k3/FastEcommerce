using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Components.Account;

internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager)
{
    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            //redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
