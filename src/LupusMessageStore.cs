using System.Buffers;

using CM.Text;

using Microsoft.Extensions.Options;

using MimeKit;

using Polly.RateLimit;

using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace LupuServ;

public class LupusMessageStore : MessageStore
{
    private readonly ServiceConfig _config;
    private readonly ILogger<LupusMessageStore> _logger;
    private readonly AsyncRateLimitPolicy<SmtpResponse> _rateLimit;

    public LupusMessageStore(ILogger<LupusMessageStore> logger, IOptions<ServiceConfig> config, AsyncRateLimitPolicy<SmtpResponse> rateLimit)
    {
        _logger = logger;
        _rateLimit = rateLimit;
        _config = config.Value;
    }

    public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction,
        ReadOnlySequence<byte> buffer,
        CancellationToken cancellationToken)
    {
        string statusUser = _config.StatusUser;
        string alarmUser = _config.AlarmUser;

        await using MemoryStream stream = new();

        SequencePosition position = buffer.GetPosition(0);
        while (buffer.TryGet(ref position, out ReadOnlyMemory<byte> memory))
        {
            await stream.WriteAsync(memory, cancellationToken);
        }

        stream.Position = 0;

        MimeMessage message = await MimeMessage.LoadAsync(stream, cancellationToken);

        string user = transaction.To.First().User;

        //
        // Send SMS in alert event only
        // 
        if (user.Equals(alarmUser, StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogInformation("Received alarm event");

            Guid apiKey = Guid.Parse(_config.ApiKey);

            string from = _config.From;

            List<string> recipients = _config.Recipients;

            try
            {
                var response = await _rateLimit.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Will send alarm SMS to the following recipients: {Recipients}",
                        string.Join(", ", recipients));

                    TextClient client = new(apiKey);

                    TextClientResult result = await client
                        .SendMessageAsync(message.TextBody, from, recipients, transaction.From.User, cancellationToken)
                        .ConfigureAwait(false);

                    if (result.statusCode == TextClientStatusCode.Ok)
                    {
                        _logger.LogInformation("Successfully sent the following message to recipients: {TextBody}",
                            message.TextBody);

                        return SmtpResponse.Ok;
                    }

                    _logger.LogError("Message delivery failed: {StatusMessage} ({StatusCode})", result.statusMessage,
                        result.statusCode);

                    return SmtpResponse.TransactionFailed;
                });

                _logger.LogDebug("Text client send result: {@Result}", response);
                
                return response;
            }
            catch (RateLimitRejectedException ex)
            {
                _logger.LogError(ex, "Rate limit hit, message not sent");
                
                // TODO: implement retry strategy?
                
                return SmtpResponse.TransactionFailed;
            }
        }

        if (user.Equals(statusUser, StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogInformation("Received status change: {TextBody}", message.TextBody);
        }
        else
        {
            _logger.LogWarning("Unknown message received (maybe a test?): {TextBody}", message.TextBody);
        }

        return SmtpResponse.Ok;
    }
}