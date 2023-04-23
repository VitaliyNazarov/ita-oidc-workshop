using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ITA.OIDC.Workshop.ResourceServer.Extensions;

internal static class ItaServiceCollectionExtensions
{
    internal static IServiceCollection AddItaAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "http://localhost:5555";
                options.MetadataAddress = "http://localhost:5555/.well-known/openid-configuration";
                options.SaveToken = false;
                options.MapInboundClaims = false;
            
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.RequireAudience = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.RoleClaimType = "role";
                options.TokenValidationParameters.NameClaimType = "name";
            });

        return services;
    }
}