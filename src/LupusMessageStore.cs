﻿using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace LupuServ
{
    public class LupusMessageStore : MessageStore
    {
        public LupusMessageStore(IConfiguration config, ILogger<Worker> logger)
        {
            Config = config;
            Logger = logger;
        }

        public IConfiguration Config { get; }

        public ILogger<Worker> Logger { get; }

        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer,
            CancellationToken cancellationToken)
        {
            var statusUser = Config.GetSection("StatusUser").Value;
            var alarmUser = Config.GetSection("AlarmUser").Value;
            
            await using var stream = new MemoryStream();

            var position = buffer.GetPosition(0);
            while (buffer.TryGet(ref position, out var memory))
            {
                await stream.WriteAsync(memory, cancellationToken);
            }

            stream.Position = 0;

            var message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);

            var user = transaction.To.First().User;

            //
            // Send SMS in alert event only
            // 
            if (user.Equals(alarmUser, StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.LogInformation("Received alarm event");

                var apiKey = Guid.Parse(Config.GetSection("ApiKey").Value);

                var from = Config.GetSection("From").Value;

                var recipients = Config.GetSection("Recipients").GetChildren().Select(e => e.Value).ToList();

                Logger.LogInformation(
                    $"Will send alarm SMS to the following recipients: {string.Join(", ", recipients)}");

                var client = new TextClient(apiKey);

                var result = await client
                    .SendMessageAsync(message.TextBody, from, recipients, transaction.From.User, cancellationToken)
                    .ConfigureAwait(false);

                if (result.statusCode == TextClientStatusCode.Ok)
                {
                    Logger.LogInformation($"Successfully sent the following message to recipients: {message.TextBody}");

                    return SmtpResponse.Ok;
                }

                Logger.LogError($"Message delivery failed: {result.statusMessage} ({result.statusCode})");

                return SmtpResponse.TransactionFailed;
            }

            if (user.Equals(statusUser, StringComparison.InvariantCultureIgnoreCase))
                Logger.LogInformation($"Received status change: {message.TextBody}");
            else
                Logger.LogWarning($"Unknown message received (maybe a test?): {message.TextBody}");

            return SmtpResponse.Ok;
        }
    }
}