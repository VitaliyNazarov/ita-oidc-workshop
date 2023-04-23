using System.Diagnostics;
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
    private readonly SignInManager<ExternalUser> _signInManager;

    public AccountController(SignInManager<ExternalUser> signInManager)
    {
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
            // Аутентифицируем пользователя и получаем детальную информацию по API
            var externalUser = new ExternalUser
            {
                Id = model.Login,
                Email = $"{model.Login}@ita.com",
                UserName = $"User{model.Login}",
                Roles = new []{ "administrator" },
                FullName = $"FullName {model.Login}"
            };
            var additionalClaims = new Claim[] { };
            
            // Указываем что пользователь externalUser является аутентифицированным 
            await _signInManager.SignInWithClaimsAsync(
                externalUser,
                model.RememberMe.GetValueOrDefault(false),
                additionalClaims);

            // Возвращаемся откуда пришли
            return RedirectToLocal(returnUrl ?? model.ReturnUrl);
        }

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Login");
    }
}
