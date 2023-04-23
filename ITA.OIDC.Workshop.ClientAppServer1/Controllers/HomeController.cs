using System.Diagnostics;
using ITA.OIDC.Workshop.ClientAppServer1.Clients;
using Microsoft.AspNetCore.Mvc;
using ITA.OIDC.Workshop.ClientAppServer1.Models;

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

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> WeatherForecast(CancellationToken cancellationToken = default)
    {
        var data = await _apiClient.WeatherForecastAsync(cancellationToken);
        var model = new WeatherForecastModel(data.Select(x =>
            new WeatherItemModel(x.Date!.Value, x.TemperatureC!.Value, x.TemperatureF!.Value, x.Summary!)).ToArray());
        return View(model); 
    }
}