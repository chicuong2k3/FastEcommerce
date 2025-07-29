using IdentityServer.Api.Requests;
using IdentityServer.Api.Responses;
using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace IdentityServer.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TwoFactorController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UrlEncoder _urlEncoder;
    private readonly ILogger<TwoFactorController> _logger;

    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    public TwoFactorController(
        UserManager<ApplicationUser> userManager,
        UrlEncoder urlEncoder,
        ILogger<TwoFactorController> logger)
    {
        _userManager = userManager;
        _urlEncoder = urlEncoder;
        _logger = logger;
    }

    [HttpGet("authenticator-setup-info")]
    public async Task<IActionResult> GetAuthenticatorSetupInfo()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var model = new GetAuthenticatorSetupInfoResponse
        {
            SharedKey = FormatKey(unformattedKey),
            AuthenticatorUri = GenerateQrCodeUri(await _userManager.GetEmailAsync(user), unformattedKey)
        };

        return Ok(model);
    }

    [HttpPost("enable")]
    public async Task<IActionResult> EnableTwoFactor(EnableAuthenticatorRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var verificationCode = request.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            return BadRequest("Verification code is invalid.");
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        _logger.LogInformation("User with ID '{UserId}' has enabled 2FA.", await _userManager.GetUserIdAsync(user));

        if (await _userManager.CountRecoveryCodesAsync(user) == 0)
        {
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        }

        return Ok("Your authenticator app has been verified.");
    }

    [HttpPost("disable")]
    public async Task<IActionResult> DisableTwoFactor()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
        {
            return BadRequest("2FA is not currently enabled.");
        }

        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            return StatusCode(500, "An unexpected error occurred while disabling 2FA.");
        }

        _logger.LogInformation("User with ID '{UserId}' has disabled 2FA.", user.Id);

        return Ok("2FA has been disabled. You can re-enable it by setting up an authenticator app.");
    }

    [HttpPost("reset-authenticator")]
    public async Task<IActionResult> ResetAuthenticator()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        await _userManager.SetTwoFactorEnabledAsync(user, false);

        await _userManager.ResetAuthenticatorKeyAsync(user);

        // Refresh sign-in session
        //await _signInManager.RefreshSignInAsync(user);

        return Ok("Your authenticator app key has been reset. You will need to configure your authenticator app using the new key.");
    }

    [HttpPost("recovery-codes")]
    public async Task<IActionResult> GenerateRecoveryCodes()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
        {
            return BadRequest("Cannot generate recovery codes because 2FA is not enabled.");
        }

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        return Ok(new
        {
            RecoveryCodes = recoveryCodes?.ToArray() ?? []
        });
    }

    private string FormatKey(string? unformattedKey)
    {
        if (string.IsNullOrEmpty(unformattedKey))
            throw new ArgumentNullException(nameof(unformattedKey));

        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }
        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string? email, string? unformattedKey)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(unformattedKey))
            throw new ArgumentNullException(nameof(email));

        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            _urlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
            _urlEncoder.Encode(email),
            unformattedKey);
    }
}
