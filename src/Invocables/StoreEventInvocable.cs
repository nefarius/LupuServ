using Coravel.Invocable;

using LupuServ.Models;
using LupuServ.Services.Interfaces;

namespace LupuServ.Invocables;

/// <summary>
///     Background job that persists a parsed event without blocking the SMTP path.
/// </summary>
public sealed class StoreEventInvocable : IInvocable, IInvocableWithPayload<EventRecord>
{
    private readonly IEventStore _eventStore;
    private readonly ILogger<StoreEventInvocable> _logger;

    public StoreEventInvocable(IEventStore eventStore, ILogger<StoreEventInvocable> logger)
    {
        _eventStore = eventStore;
        _logger = logger;
    }

    /// <inheritdoc />
    public EventRecord Payload { get; set; } = null!;

    /// <inheritdoc />
    public async Task Invoke()
    {
        _logger.LogDebug("Persisting queued {Kind} event", Payload.Kind);
        await _eventStore.StoreAsync(Payload);
        _logger.LogDebug("Queued {Kind} event inserted into DB", Payload.Kind);
    }
}
