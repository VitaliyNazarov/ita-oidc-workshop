using System.Security.Claims;
using ITA.OIDC.Workshop.AuthServer.DataAccess;
using ITA.OIDC.Workshop.AuthServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITA.OIDC.Workshop.AuthServer.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<ExternalUser> _userManager;
    private readonly SignInManager<ExternalUser> _signInManager;

    public AccountController(
        UserManager<ExternalUser> userManager,
        SignInManager<ExternalUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            var externalUser = await GetCurrentUserAsync();
            var additionalClaims = new Claim[] { };
            
            await _signInManager.SignInWithClaimsAsync(
                externalUser,
                model.RememberMe.GetValueOrDefault(false),
                additionalClaims);
        }

        return View(model);
    }

    private async Task<ExternalUser> GetCurrentUserAsync()
    {
        return await _userManager.GetUserAsync(User);
    }
}
