using System.Net.Http.Headers;
using System.Text;

namespace LupuServ.Util;

internal class BasicAuthHandler : HttpClientHandler
{
    private readonly string _password;
    private readonly string _username;

    public BasicAuthHandler(string username,
        string password)
    {
        _username = username;
        _password = password;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(
                $"{_username}:{_password}")));

        return base.SendAsync(request, cancellationToken);
    }
}