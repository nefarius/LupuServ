using System.Buffers;

using LupuServ.Models;
using LupuServ.Services.Interfaces;
using LupuServ.Services.Web;

using Microsoft.Extensions.Options;

using MimeKit;

using MongoDB.Entities;

using Polly.RateLimit;

using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

// ReSharper disable InvertIf

namespace LupuServ;

/// <summary>
///     Receives and processes incoming mail messages.
/// </summary>
public class LupusMessageStore : MessageStore
{
    private readonly ServiceConfig _config;

    private readonly ILogger<LupusMessageStore> _logger;
    private readonly IMessageGateway _messageGateway;
    private readonly AsyncRateLimitPolicy<SmtpResponse> _rateLimit;

    public LupusMessageStore(ILogger<LupusMessageStore> logger, IOptions<ServiceConfig> config,
        AsyncRateLimitPolicy<SmtpResponse> rateLimit, IMessageGateway messageGateway,
        IGotifySystemApi? gotifySystemApi = null, IGotifyAlarmApi? gotifyAlarmApi = null,
        IGotifyStatusApi? gotifyStatusApi = null)
    {
        _logger = logger;
        _rateLimit = rateLimit;
        _messageGateway = messageGateway;
        _gotifySystemApi = gotifySystemApi;
        _gotifyAlarmApi = gotifyAlarmApi;
        _gotifyStatusApi = gotifyStatusApi;
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

        // matches alarm message
        if (user.Equals(alarmUser, StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogInformation("Received alarm event");

            try
            {
                if (AlarmEvent.TryParse(message.TextBody, out AlarmEvent? alarmEvent) &&
                    alarmEvent is not null)
                {
                    await alarmEvent.SaveAsync(cancellation: cancellationToken);
                    _logger.LogDebug("Alarm event inserted into DB");

                    await _gotifyAlarmApi.SendMessage(_config, message.TextBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse alarm event");
            }

            string from = _config.From;
            List<string> recipients = _config.Recipients;

            try
            {
                SmtpResponse? response = await _rateLimit.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Will send alarm SMS to the following recipients: {Recipients}",
                        string.Join(", ", recipients));

                    bool result =
                        await _messageGateway.SendMessage(message.TextBody, from, recipients, cancellationToken);

                    return result ? SmtpResponse.Ok : SmtpResponse.TransactionFailed;
                });

                _logger.LogDebug("Text client send result: {@Result}", response);

                return response;
            }
            catch (RateLimitRejectedException ex)
            {
                _logger.LogError(ex, "Rate limit hit, message not sent");

                await _gotifySystemApi.SendMessage(_config, "Alarm rate limit hit", ex.ToString());

                return SmtpResponse.TransactionFailed;
            }
        }

        // matches status message
        if (user.Equals(statusUser, StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogInformation("Received status change: {TextBody}", message.TextBody);

            await _gotifyStatusApi.SendMessage(_config, message.TextBody);

            if (ZoneMobilityEvent.TryParse(message.TextBody, out ZoneMobilityEvent? zoneMobilityEvent) &&
                zoneMobilityEvent is not null)
            {
                await zoneMobilityEvent.SaveAsync(cancellation: cancellationToken);
                _logger.LogDebug("Zone status event inserted into DB");
            }
            else if (PerimeterStatusEvent.TryParse(message.TextBody, out PerimeterStatusEvent? perimeterEvent) &&
                     perimeterEvent is not null)
            {
                await perimeterEvent.SaveAsync(cancellation: cancellationToken);
                _logger.LogDebug("Perimeter status event inserted into DB");
            }
            else if (SensorStatusEvent.TryParse(message.TextBody, out SensorStatusEvent? statusEvent) &&
                     statusEvent is not null)
            {
                await statusEvent.SaveAsync(cancellation: cancellationToken);
                _logger.LogDebug("Sensor status event inserted into DB");

                // send critical events to alarm subscribers
                if (statusEvent.EventType.IsCritical)
                {
                    string from = _config.From;
                    List<string> recipients = _config.Recipients;

                    try
                    {
                        SmtpResponse? response = await _rateLimit.ExecuteAsync(async () =>
                        {
                            _logger.LogInformation("Will send status SMS to the following recipients: {Recipients}",
                                string.Join(", ", recipients));

                            bool result =
                                await _messageGateway.SendMessage(message.TextBody, from, recipients,
                                    cancellationToken);

                            return result ? SmtpResponse.Ok : SmtpResponse.TransactionFailed;
                        });

                        _logger.LogDebug("Text client send result: {@Result}", response);

                        return response;
                    }
                    catch (RateLimitRejectedException ex)
                    {
                        _logger.LogError(ex, "Rate limit hit, message not sent");

                        await _gotifySystemApi.SendMessage(_config, "Status rate limit hit", ex.ToString());

                        return SmtpResponse.TransactionFailed;
                    }
                }
            }
        }
        else
        {
            _logger.LogWarning("Unknown message received (maybe a test?): {TextBody}", message.TextBody);

            await _gotifySystemApi.SendMessage(_config, "Unknown message received", message.TextBody);
        }

        return SmtpResponse.Ok;
    }

    #region Gotify instances

    private readonly IGotifyAlarmApi? _gotifyAlarmApi;
    private readonly IGotifyStatusApi? _gotifyStatusApi;
    private readonly IGotifySystemApi? _gotifySystemApi;

    #endregion
}