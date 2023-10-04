using System.Net.Http.Headers;
using System.Text;

using Microsoft.Extensions.Options;

namespace LupuServ.Services.Web;

/// <summary>
///     Injects authentication header into Refit requests.
/// </summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ServiceConfig _config;

    public AuthHeaderHandler(IOptions<ServiceConfig> config)
    {
        _config = config.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string? username = _config.ClickSend.Username;
        string? password = _config.ClickSend.Token;

        string authenticationString = $"{username}:{password}";
        string token = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}