﻿using ITA.OIDC.Workshop.AuthServer.DataAccess;
using Microsoft.AspNetCore.Identity;
using ITA.OIDC.Workshop.AuthServer.Oidc;
using OpenIddict.Abstractions;

namespace ITA.OIDC.Workshop.AuthServer.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddItaOidcServer(this IServiceCollection services, string issuer)
    {
        services.AddItaDataAccess();
        
        // 1. Регистрация Identity компонент
        services
            .AddIdentity<ExternalUser, ExternalRole>() // Свой класс пользователей и ролей
            .AddUserStore<ExternalUserStore>()         // Свой класс - хранилище пользователей
            .AddRoleStore<ExternalRoleStore>();        // Свой класс - хранилище ролей
        
        // Конфигурируем имена клеймов
        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Username;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
        });

        // 2. Регистрация OpenIddict компонент
        services.AddOpenIddict()
            .AddCore(builder =>
            {
                builder
                    .UseEntityFrameworkCore()
                    .UseDbContext<OidcDbContext>();
            })
            .AddServer(builder =>
            {
                // 3. Устанавливаем издателя
                builder.SetIssuer(new Uri(issuer));

                // 4. Какие endpoints поддерживает сервер?
                builder.SetAuthorizationEndpointUris("connect/authorize")
                    .SetTokenEndpointUris("connect/token")
                    .SetUserinfoEndpointUris("connect/userinfo");

                // 5. Какие flows поддерживает сервер?
                builder.AllowAuthorizationCodeFlow();
                
                // 6. Какие scopes поддерживает сервер?
                builder.RegisterScopes(
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Address,
                    OpenIddictConstants.Scopes.Phone,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.OfflineAccess);
                
                builder.UseAspNetCore()
                    .EnableStatusCodePagesIntegration()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableUserinfoEndpointPassthrough();

                // 7.Регистрируем сертификаты для шифрования и подписи.
                // Для демо используем автогенерацию сертификатов.
                builder
                    .AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();
                
                // Для демо отключим требование HTTPS
                builder
                    .UseAspNetCore()
                    .DisableTransportSecurityRequirement();
                
                builder.DisableAccessTokenEncryption();
            })

            // Регистрируем компоненты валидации.
            .AddValidation(options =>
            {
                options.SetIssuer(issuer);
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        // Регистрация клиентов
        services.AddHostedService<ClientsRegistrar>();
        
        return services;
    }
}