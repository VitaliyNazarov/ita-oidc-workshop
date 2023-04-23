using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace ITA.OIDC.Workshop.ClientAppServer1.Clients;

public partial class ApiClient
{
    private readonly IHttpContextAccessor _contextAccessor;
    
    public ApiClient(IHttpContextAccessor httpContextAccessor, HttpClient client)
        :this(client)
    {
        _contextAccessor = httpContextAccessor;
    }
    
    #region Overrides of BaseClient

    protected override async Task PrepareRequestAsync(
        HttpClient client,
        HttpRequestMessage message,
        StringBuilder urlBuilder,
        CancellationToken cancellationToken)
    {
        var accessToken = await _contextAccessor.HttpContext?.GetTokenAsync("access_token")!;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new AuthenticationException("Access token is not found");
        }
        
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    protected override Task PrepareRequestAsync(
        HttpClient client,
        HttpRequestMessage message,
        string url,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task ProcessResponseAsync(
        HttpClient client,
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion
}