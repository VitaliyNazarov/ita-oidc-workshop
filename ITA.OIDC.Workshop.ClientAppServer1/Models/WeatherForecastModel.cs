namespace ITA.OIDC.Workshop.ClientAppServer1.Models;

public record WeatherForecastModel(ICollection<WeatherItemModel> items);

public record WeatherItemModel(DateTime Date, int TemperatureC, int TemperatureF, string Summary);