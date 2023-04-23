using ITA.OIDC.Workshop.ClientAppServer1.Clients;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ITA.OIDC.Workshop.ClientAppServer1.Extensions;

internal static class ItaServiceCollectionExtensions
{
    internal const string ItaAuthenticationScheme = "ITA";

    internal static IServiceCollection AddItaApiClient(
        this IServiceCollection services,
        string apiUrl)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient<IApiClient, ApiClient>((client, provider) =>
        {
            using var scope = provider.CreateScope();
            var contextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            client.BaseAddress = new Uri(apiUrl);
            return new ApiClient(contextAccessor, client);
        });
        return services;
    }

    internal static IServiceCollection AddItaAuthentication(
        this IServiceCollection services,
        string cookieName,
        string cookiePath,
        int cookieLifetime,
        string loginPageUrl)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = ItaAuthenticationScheme; // 1
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = cookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.Path = cookiePath;
                options.ExpireTimeSpan = TimeSpan.FromDays(cookieLifetime);
                options.LoginPath = new PathString(loginPageUrl);
            })
            .AddOpenIdConnect(ItaAuthenticationScheme, options => // 2
            {
                options.Authority = "http://localhost:5555";
                options.ClientId = "ITA.ClientAppServer1";
                options.ClientSecret = "4855CB0F-5310-4E22-8151-EA30F8301B91";

                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "role";
                options.TokenValidationParameters.AuthenticationType = ItaAuthenticationScheme;

                options.ResponseType = OpenIdConnectResponseType.Code;
                
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("roles");
                options.Scope.Add("phone");
                options.Scope.Add("email");
                
                options.CallbackPath = new PathString("/signin-callback");

                options.ClaimsIssuer = ItaAuthenticationScheme;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = false;
                options.MapInboundClaims = false;
                
                options.NonceCookie.SetCookieBuilderSettings(".ITA.Nonce.", SameSiteMode.Unspecified);
                options.CorrelationCookie.SetCookieBuilderSettings(".ITA.Correlation.", SameSiteMode.Unspecified);
            });
        
        return services;
    }
    
    private static void SetCookieBuilderSettings(this CookieBuilder builder, string? name, SameSiteMode? sameSite)
    {
        builder.Name = name ?? builder.Name;
        builder.SameSite = sameSite ?? builder.SameSite;
    }
}