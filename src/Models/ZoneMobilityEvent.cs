using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Ardalis.SmartEnum;

using MongoDB.Entities;

namespace LupuServ.Models;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed class ZoneMobilityEventType : SmartEnum<ZoneMobilityEventType>
{
    public static readonly ZoneMobilityEventType DoorContactOpen = new("DC Open", 1);
    public static readonly ZoneMobilityEventType DoorContactClose = new("DC Close", 2);
    public static readonly ZoneMobilityEventType InfraRedActivity = new("IR Activity", 3);

    private ZoneMobilityEventType(string name, int value) : base(name, value)
    {
    }
}

/// <summary>
///     Represents a zone (sensor change) status event.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed partial class ZoneMobilityEvent : Entity
{
    /// <summary>
    ///     Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    ///     Gets the Zone ID (Sensor number).
    /// </summary>
    public int ZoneId { get; private set; }

    /// <summary>
    ///     Gets the sensor name.
    /// </summary>
    public string SensorName { get; private set; } = null!;

    /// <summary>
    ///     Gets the type of this event.
    /// </summary>
    public ZoneMobilityEventType EventType { get; private set; } = null!;

    [GeneratedRegex(@"^Zone:(\d*) ([a-zA-Z\u00F0-\u02AF0-9 _.-]*), ([a-zA-Z0-9 _.-]*) - Mobility$")]
    private static partial Regex ZoneMobilityRegex();

    public static bool TryParse(string message, out ZoneMobilityEvent? parsedEvent)
    {
        Match match = ZoneMobilityRegex().Match(message);

        if (!match.Success)
        {
            parsedEvent = null;
            return false;
        }

        int zoneId = int.Parse(match.Groups[1].Value);
        string sensorName = match.Groups[2].Value;

        if (!ZoneMobilityEventType.TryFromName(match.Groups[3].Value, out ZoneMobilityEventType? eventType))
        {
            parsedEvent = null;
            return false;
        }

        parsedEvent = new ZoneMobilityEvent
        {
            CreatedAt = DateTime.Now, ZoneId = zoneId, SensorName = sensorName, EventType = eventType
        };

        return true;
    }
}