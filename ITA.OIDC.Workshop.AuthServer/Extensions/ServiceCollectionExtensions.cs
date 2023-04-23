using ITA.OIDC.Workshop.AuthServer.DataAccess;
using OpenIddict.Abstractions;

namespace ITA.OIDC.Workshop.AuthServer.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddItaOidcServer(this IServiceCollection services, string issuer)
    {
        // 1. Регистрация контекста с которым будет работать BL OpenIddict
        services.AddDbContext<OidcDbContext>();
        
        // 2. Регистрация Identity компонент
        services
            .AddIdentity<ExternalUser, ExternalRole>() // Свой класс пользователей и ролей
            .AddUserStore<ExternalUserStore>()         // Свой класс - хранилище пользователей
            .AddRoleStore<ExternalRoleStore>();        // Свой класс - хранилище ролей

        // 3. Регистрация OpenIddict компонент
        services.AddOpenIddict()
            .AddCore(builder =>
            {
                builder
                    .UseEntityFrameworkCore()
                    .UseDbContext<OidcDbContext>();
            })
            .AddServer(builder =>
            {
                // Устанавливаем издателя
                builder.SetIssuer(new Uri(issuer));

                // Какие endpoints поддерживает сервер?
                builder.SetAuthorizationEndpointUris("connect/authorize")
                    .SetLogoutEndpointUris("connect/logout")
                    .SetTokenEndpointUris("connect/token")
                    .SetUserinfoEndpointUris("connect/userinfo");

                // Какие flows поддерживает сервер?
                builder.AllowAuthorizationCodeFlow();
                
                // Какие scopes поддерживает сервер?
                builder.RegisterScopes(
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Address,
                    OpenIddictConstants.Scopes.Phone,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.OfflineAccess);

                // Регистриуем специфичные для ASP.NET Core зависимости.
                builder.UseAspNetCore()
                    .EnableStatusCodePagesIntegration()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableLogoutEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableUserinfoEndpointPassthrough();

                // Регистрируем сертификаты для шифрования и подписи.
                // Для демо используем автогенерацию сертификатов.
                builder
                    .AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();
                
                // Для демо отключим требование HTTPS
                builder
                    .UseAspNetCore()
                    .DisableTransportSecurityRequirement();
            })

            // Регистрируем компоненты валидации.
            .AddValidation(options =>
            {
                options.SetIssuer(issuer);
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        
        return services;
    }
}