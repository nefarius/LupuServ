using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

// ReSharper disable InvertIf

namespace LupuServ.Models;

/// <summary>
///     Represents an alarm or sabotage event.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed partial class AlarmEvent
{
    /// <summary>
    ///     Creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    ///     Gets the Zone ID (Sensor number). Not every message includes a zone identifier.
    /// </summary>
    public int? ZoneId { get; private set; }

    /// <summary>
    ///     Gets the sensor name.
    /// </summary>
    public string SensorName { get; private set; } = null!;

    /// <summary>
    ///     Gets the type of this event.
    /// </summary>
    public AlarmEventType EventType { get; private set; } = null!;

    [GeneratedRegex(@"^Zone:(\d*) ([a-zA-Z\u00F0-\u02AF0-9 _.-]*), (Sabotage Alarm)$")]
    private static partial Regex SabotageAlarmRegex();

    [GeneratedRegex(@"^Zone:(\d*) ([a-zA-Z\u00F0-\u02AF0-9 _.-]*), (Sabotage Alarm deaktiviert)$")]
    private static partial Regex SabotageAlarmResolvedRegex();

    [GeneratedRegex(@"^Zone:(\d*) ([a-zA-Z\u00F0-\u02AF0-9 _.-]*), (Innenbereich Alarm)$")]
    private static partial Regex InteriorAlarmRegex();

    [GeneratedRegex(@"^(Innenbereich Alarm gemeldet von Sensor) ([a-zA-Z\u00F0-\u02AF0-9 _.-]*)$")]
    private static partial Regex InteriorAlarmSensorReportedRegex();

    [GeneratedRegex(@"^(Sabotage Alarm gemeldet von Sensor) ([a-zA-Z\u00F0-\u02AF0-9 _.-]*)$")]
    private static partial Regex SabotageAlarmSensorReportedRegex();

    [GeneratedRegex(@"^(Sabotage Alarm deaktiviert gemeldet von Sensor) ([a-zA-Z\u00F0-\u02AF0-9 _.-]*)$")]
    private static partial Regex SabotageAlarmDisabledSensorReportedRegex();

    /// <summary>
    ///     Maps this event to a flat store row.
    /// </summary>
    public EventRecord ToRecord(string? rawMessage) =>
        new(CreatedAt, "Alarm", EventType.Name, EventType.Value, ZoneId, SensorName, null, rawMessage);

    public static bool TryParse(string message, out AlarmEvent? parsedEvent)
    {
        parsedEvent = new AlarmEvent { CreatedAt = DateTimeOffset.UtcNow };

        Match match = SabotageAlarmRegex().Match(message);

        if (match.Success)
        {
            parsedEvent.ZoneId = int.Parse(match.Groups[1].Value);
            parsedEvent.SensorName = match.Groups[2].Value;
            parsedEvent.EventType = AlarmEventType.FromName(match.Groups[3].Value);

            return true;
        }

        match = SabotageAlarmResolvedRegex().Match(message);

        if (match.Success)
        {
            parsedEvent.ZoneId = int.Parse(match.Groups[1].Value);
            parsedEvent.SensorName = match.Groups[2].Value;
            parsedEvent.EventType = AlarmEventType.FromName(match.Groups[3].Value);

            return true;
        }

        match = InteriorAlarmRegex().Match(message);

        if (match.Success)
        {
            parsedEvent.ZoneId = int.Parse(match.Groups[1].Value);
            parsedEvent.SensorName = match.Groups[2].Value;
            parsedEvent.EventType = AlarmEventType.FromName(match.Groups[3].Value);

            return true;
        }

        match = InteriorAlarmSensorReportedRegex().Match(message);

        if (match.Success)
        {
            parsedEvent.EventType = AlarmEventType.FromName(match.Groups[1].Value);
            parsedEvent.SensorName = match.Groups[2].Value;
        }

        match = SabotageAlarmSensorReportedRegex().Match(message);

        if (match.Success)
        {
            parsedEvent.EventType = AlarmEventType.FromName(match.Groups[1].Value);
            parsedEvent.SensorName = match.Groups[2].Value;
        }

        match = SabotageAlarmDisabledSensorReportedRegex().Match(message);

        if (match.Success)
        {
            parsedEvent.EventType = AlarmEventType.FromName(match.Groups[1].Value);
            parsedEvent.SensorName = match.Groups[2].Value;
        }

        throw new NotImplementedException("Couldn't parse event type");
    }
}
