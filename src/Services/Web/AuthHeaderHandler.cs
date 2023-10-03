using System.Net.Http.Headers;

using Microsoft.Extensions.Options;

namespace LupuServ.Services.Web;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ServiceConfig _config;

    public AuthHeaderHandler(IOptions<ServiceConfig> config)
    {
        _config = config.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = "";

        //potentially refresh token here if it has expired etc.

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //request.Headers.Add("X-Tenant-Id", tenantProvider.GetTenantId());

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
