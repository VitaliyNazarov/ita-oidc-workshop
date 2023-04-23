using ITA.OIDC.Workshop.ClientAppServer2.Clients;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ITA.OIDC.Workshop.ClientAppServer2.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddItaApiClient(
        this IServiceCollection services,
        string apiUrl)
    {
        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiUrl);
        });
        return services;
    }

    internal static IServiceCollection AddItaCookieAuthentication(
        this IServiceCollection services,
        string cookieName,
        string cookiePath,
        int cookieLifetime,
        string loginPageUrl)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = cookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.Path = cookiePath;
                options.ExpireTimeSpan  = TimeSpan.FromDays(cookieLifetime);
                options.LoginPath = new PathString(loginPageUrl);
            });
        return services;
    }
}