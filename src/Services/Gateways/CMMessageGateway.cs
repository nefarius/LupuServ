using System.Diagnostics.CodeAnalysis;

using CM.Text;

using LupuServ.Services.Interfaces;

using Microsoft.Extensions.Options;

namespace LupuServ.Services.Gateways;

/// <summary>
///     Sends SMS via https://www.cm.com/
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class CMMessageGateway : IMessageGateway
{
    private readonly ILogger<CMMessageGateway> _logger;
    private readonly TextClient _textClient;

    public CMMessageGateway(IOptions<ServiceConfig> config, ILogger<CMMessageGateway> logger)
    {
        _logger = logger;

        _textClient = new TextClient(config.Value.CM!.ApiKey);
    }

    public async Task<bool> SendMessage(string body, string from, IEnumerable<string> recipients,
        CancellationToken token = default)
    {
        _logger.LogDebug("Sending message");
        
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