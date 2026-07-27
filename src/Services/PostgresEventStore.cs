using LupuServ.Models;
using LupuServ.Services.Interfaces;

using Npgsql;

namespace LupuServ.Services;

/// <summary>
///     PostgreSQL-backed <see cref="IEventStore" />.
/// </summary>
public sealed class PostgresEventStore : IEventStore
{
    private const string EnsureSchemaSql = """
                                           CREATE TABLE IF NOT EXISTS events (
                                               id               bigserial PRIMARY KEY,
                                               created_at       timestamptz NOT NULL,
                                               kind             text        NOT NULL,
                                               event_type_name  text        NOT NULL,
                                               event_type_value integer     NOT NULL,
                                               zone_id          integer     NULL,
                                               sensor_name      text        NULL,
                                               username         text        NULL,
                                               raw_message      text        NULL
                                           );
                                           CREATE INDEX IF NOT EXISTS ix_events_created_at ON events (created_at DESC);
                                           """;

    private const string InsertSql = """
                                     INSERT INTO events (
                                         created_at,
                                         kind,
                                         event_type_name,
                                         event_type_value,
                                         zone_id,
                                         sensor_name,
                                         username,
                                         raw_message
                                     ) VALUES (
                                         @created_at,
                                         @kind,
                                         @event_type_name,
                                         @event_type_value,
                                         @zone_id,
                                         @sensor_name,
                                         @username,
                                         @raw_message
                                     );
                                     """;

    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<PostgresEventStore> _logger;

    public PostgresEventStore(NpgsqlDataSource dataSource, ILogger<PostgresEventStore> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task EnsureSchemaAsync(CancellationToken cancellationToken = default)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using NpgsqlCommand command = new(EnsureSchemaSql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation("Database schema ensured");
    }

    /// <inheritdoc />
    public async Task StoreAsync(EventRecord record, CancellationToken cancellationToken = default)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using NpgsqlCommand command = new(InsertSql, connection);

        // Npgsql only accepts offset 0 for timestamptz
        command.Parameters.AddWithValue("created_at", record.CreatedAt.ToUniversalTime());
        command.Parameters.AddWithValue("kind", record.Kind);
        command.Parameters.AddWithValue("event_type_name", record.EventTypeName);
        command.Parameters.AddWithValue("event_type_value", record.EventTypeValue);
        command.Parameters.AddWithValue("zone_id", (object?)record.ZoneId ?? DBNull.Value);
        command.Parameters.AddWithValue("sensor_name", (object?)record.SensorName ?? DBNull.Value);
        command.Parameters.AddWithValue("username", (object?)record.Username ?? DBNull.Value);
        command.Parameters.AddWithValue("raw_message", (object?)record.RawMessage ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogDebug("Stored {Kind} event ({EventTypeName})", record.Kind, record.EventTypeName);
    }
}
