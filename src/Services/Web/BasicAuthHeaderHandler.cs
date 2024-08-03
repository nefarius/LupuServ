using Microsoft.Extensions.Options;

namespace LupuServ.Services.Web;

/// <summary>
///     Injects basic authentication header into Refit requests.
/// </summary>
internal abstract class BasicAuthHeaderHandler : DelegatingHandler
{
    protected readonly ServiceConfig _config;

    protected BasicAuthHeaderHandler(IOptions<ServiceConfig> config)
    {
        _config = config.Value;
    }
}