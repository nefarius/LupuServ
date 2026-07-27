namespace LupuServ.Models;

/// <summary>
///     Flat row shape written to the events table.
/// </summary>
public sealed record EventRecord(
    DateTimeOffset CreatedAt,
    string Kind,
    string EventTypeName,
    int EventTypeValue,
    int? ZoneId,
    string? SensorName,
    string? Username,
    string? RawMessage);
