using System.Diagnostics;
using ITA.OIDC.Workshop.ClientAppServer1.Clients;
using Microsoft.AspNetCore.Mvc;
using ITA.OIDC.Workshop.ClientAppServer1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ITA.OIDC.Workshop.ClientAppServer1.Controllers;

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
    [Authorize]
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