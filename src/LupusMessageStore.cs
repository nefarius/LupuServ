using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CM.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using MimeKit;

using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace LupuServ;

public class LupusMessageStore : MessageStore
{
    private readonly IConfiguration _config;

    private readonly ILogger<Worker> _logger;

    public LupusMessageStore(IConfiguration config, ILogger<Worker> logger)
    {
        _config = config;
        _logger = logger;
    }

    public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction,
        ReadOnlySequence<byte> buffer,
        CancellationToken cancellationToken)
    {
        string statusUser = _config.GetSection("StatusUser").Value;
        string alarmUser = _config.GetSection("AlarmUser").Value;

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

            Guid apiKey = Guid.Parse(_config.GetSection("ApiKey").Value);

            string from = _config.GetSection("From").Value;

            List<string> recipients = _config.GetSection("Recipients").GetChildren().Select(e => e.Value).ToList();

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