using System.Text;

namespace ITA.OIDC.Workshop.ClientAppServer1.Clients;

public abstract class BaseApiClient
{
    protected abstract Task PrepareRequestAsync(
        HttpClient client,
        HttpRequestMessage message,
        StringBuilder urlBuilder,
        CancellationToken cancellationToken);
        
    protected abstract Task PrepareRequestAsync(
        HttpClient client,
        HttpRequestMessage message,
        string url,
        CancellationToken cancellationToken);

    protected abstract Task ProcessResponseAsync(
        HttpClient client,
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken);
}