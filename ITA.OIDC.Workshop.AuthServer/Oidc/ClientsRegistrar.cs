using OpenIddict.Abstractions;

namespace ITA.OIDC.Workshop.AuthServer.Oidc;

/// <summary>
/// Регистратор клиентов. Вызывается на старте приложения.
/// </summary>
internal sealed class ClientsRegistrar : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ClientsRegistrar(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    #region Implementation of IHostedService

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        // Первый клиент
        await RegisterApplicationIfNotExists(
            scope.ServiceProvider,
            clientId: "ITA.ClientAppServer1",
            clientSecret: "4855CB0F-5310-4E22-8151-EA30F8301B91",
            displayName: "ITA Client App 1", 
            loginCallback: "http://localhost:5091/ClientApp1/signin-callback",
            logoutCallback: "http://localhost:5091/ClientApp1");
        
        // Второй клиент
        await RegisterApplicationIfNotExists(
            scope.ServiceProvider,
            clientId: "ITA.ClientAppServer2",
            clientSecret: "B570E013-E8CD-486E-9E1C-46E59E7A8FAD",
            displayName: "ITA Client App 2", 
            loginCallback: "http://localhost:5092/ClientApp2/signin-callback",
            logoutCallback: "http://localhost:5092/ClientApp2");
        
        // Postman - тестовый клиент
        await RegisterApplicationIfNotExists(
            scope.ServiceProvider,
            clientId: "Postman",
            clientSecret: "A03740BD-1643-41C9-8B7E-8E6039083930",
            displayName: "Postman Test Client", 
            loginCallback: "https://oauth.pstmn.io/v1/callback",
            logoutCallback: "https://oauth.pstmn.io/v1/callback");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion

    /// <summary>
    /// Непосредственная регистрация приложения.
    /// </summary>
    static async Task RegisterApplicationIfNotExists(
        IServiceProvider provider,
        string clientId,
        string clientSecret,
        string displayName,
        string loginCallback,
        string logoutCallback)
    {
        var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();
        
        if (await manager.FindByClientIdAsync(clientId) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                DisplayName = displayName,
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                Type = OpenIddictConstants.ClientTypes.Confidential,
                RedirectUris =
                {
                    new Uri(loginCallback) 
                },
                PostLogoutRedirectUris =
                {
                    new Uri(logoutCallback),
                },
                Permissions =
                {
                    // Разрешенные endpotins
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    
                    // Разрешенные flow
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,

                    // Разрешенный тип ответа
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    
                    // Разрешенные scopes
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Phone,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    OpenIddictConstants.Permissions.Scopes.Address
                }
            });
        }
    }
}