using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Ardalis.SmartEnum;

using MongoDB.Entities;

// ReSharper disable InvertIf

namespace LupuServ.Models;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed class AlarmEventType : SmartEnum<AlarmEventType>
{
    public static readonly AlarmEventType SabotageAlarm = new("Sabotage Alarm", 1);
    public static readonly AlarmEventType SabotageAlarmDisabled = new("Sabotage Alarm deaktiviert", 2);
    public static readonly AlarmEventType InteriorAlarm = new("Innenbereich Alarm", 3);

    public static readonly AlarmEventType
        InteriorAlarmSensorReported = new("Innenbereich Alarm gemeldet von Sensor", 4);

    public static readonly AlarmEventType SabotageAlarmSensorReported = new("Sabotage Alarm gemeldet von Sensor", 5);

    public static readonly AlarmEventType SabotageAlarmDisabledSensorReported =
        new("Sabotage Alarm deaktiviert gemeldet von Sensor", 6);

    private AlarmEventType(string name, int value) : base(name, value)
    {
    }
}

/// <summary>
///     Represents an alarm or sabotage event.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed partial class AlarmEvent : Entity
{
    /// <summary>
    ///     Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

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

    public static bool TryParse(string message, out AlarmEvent? parsedEvent)
    {
        parsedEvent = new AlarmEvent { CreatedAt = DateTime.Now };

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