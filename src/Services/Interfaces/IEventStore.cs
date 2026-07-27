using LupuServ.Models;

namespace LupuServ.Services.Interfaces;

/// <summary>
///     Append-only store for alarm and status events.
/// </summary>
public interface IEventStore
{
    /// <summary>
    ///     Ensures the events table and indexes exist.
    /// </summary>
    Task EnsureSchemaAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Inserts a single event row.
    /// </summary>
    Task StoreAsync(EventRecord record, CancellationToken cancellationToken = default);
}
