using System.Diagnostics.CodeAnalysis;

using CM.Text;

using Microsoft.Extensions.Options;

namespace LupuServ.Services.Gateways;

/// <summary>
///     Sends SMS via https://www.cm.com/
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class CMMessageGateway : IMessageGateway
{
    private readonly ILogger<CMMessageGateway> _logger;
    private readonly TextClient _textClient;

    public CMMessageGateway(IOptions<ServiceConfig> config, ILogger<CMMessageGateway> logger)
    {
        _logger = logger;

        ServiceConfig config1 = config.Value;
        Guid apiKey = Guid.Parse(config1.ApiKey);
        _textClient = new TextClient(apiKey);
    }

    public async Task<bool> SendMessage(string body, string from, IEnumerable<string> recipients,
        CancellationToken token)
    {
        TextClientResult result = await _textClient
            .SendMessageAsync(body, from, recipients, string.Empty, token)
            .ConfigureAwait(false);

        if (result.statusCode != TextClientStatusCode.Ok)
        {
            _logger.LogError("Message delivery failed, status: {@Status}", result);
            return false;
        }

        _logger.LogDebug("Successfully sent {TextBody} to {@Recipients}", body, recipients);

        return true;
    }
}