using IdentityServer.Api.Requests;
using IdentityServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePersonalData([FromBody] DeleteAccountRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var requirePassword = await _userManager.HasPasswordAsync(user);
        if (requirePassword)
        {
            var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!validPassword)
            {
                return BadRequest("Incorrect password.");
            }
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return StatusCode(500, "Unexpected error occurred deleting user.");
        }

        return NoContent();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return BadRequest("You must set a password before changing it.");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        return NoContent();
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (hasPassword)
            return BadRequest("User already has a password.");

        var result = await _userManager.AddPasswordAsync(user, request.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return NoContent();
    }

}
