using System.Diagnostics;
using System.Security.Claims;
using ITA.OIDC.Workshop.ClientAppServer2.Clients;
using Microsoft.AspNetCore.Mvc;
using ITA.OIDC.Workshop.ClientAppServer2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ITA.OIDC.Workshop.ClientAppServer2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IApiClient _apiClient;

    public HomeController(ILogger<HomeController> logger, IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginModel
        {
            ReturnUrl = returnUrl
        });
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        //TODO: Проверяем логин и пароль пользователя.

        // Пользователь аутентифицирован. По данным пользователя создаем ClaimsPrincipal.
        var claims = new List<Claim>();
        claims.Add(new Claim("sub", model.Login));
        claims.Add(new Claim("name", model.Login));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", null);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
        
        return Redirect(model.ReturnUrl ?? "/");
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize]
    public async Task<IActionResult> WeatherForecast(CancellationToken cancellationToken = default)
    {
        var data = await _apiClient.WeatherForecastAsync(cancellationToken);
        var model = new WeatherForecastModel(data.Select(x =>
            new WeatherItemModel(x.Date!.Value, x.TemperatureC!.Value, x.TemperatureF!.Value, x.Summary!)).ToArray());
        return View(model); 
    }
}