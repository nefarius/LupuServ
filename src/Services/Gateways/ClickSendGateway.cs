using LupuServ.Models.Web;
using LupuServ.Services.Interfaces;
using LupuServ.Services.Web;

namespace LupuServ.Services.Gateways;

public sealed class ClickSendGateway : IMessageGateway
{
    private readonly ILogger<ClickSendGateway> _logger;
    private readonly IClickSendApi _sendApi;

    public ClickSendGateway(ILogger<ClickSendGateway> logger, IClickSendApi sendApi)
    {
        _logger = logger;
        _sendApi = sendApi;
    }

    public async Task<bool> SendMessage(string body, string from, IEnumerable<string> recipients,
        CancellationToken token = default)
    {
        _logger.LogDebug("Sending message");
        
        SmsRequest request = new()
        {
            Messages = recipients.Select(to => new SmsRequestMessage { Body = body, Source = from, To = to })
                .ToList()
        };

        SmsResponse response = await _sendApi.SendSms(request);

        _logger.LogDebug("Got response {@Response}", response);

        return response.ResponseCode == "SUCCESS";
    }
}