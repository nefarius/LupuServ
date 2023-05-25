using System.Text.RegularExpressions;

using Ardalis.SmartEnum;

using MongoDB.Entities;

namespace LupuServ.Models;

public sealed class ZoneMobilityEventType : SmartEnum<ZoneMobilityEventType>
{
    public static ZoneMobilityEventType DoorContactOpen = new("DC Open", 1);
    public static ZoneMobilityEventType DoorContactClose = new("DC Close", 2);
    public static ZoneMobilityEventType InfraRedActivity = new("IR Activity", 3);

    private ZoneMobilityEventType(string name, int value) : base(name, value)
    {
    }
}

public sealed partial class ZoneMobilityEvent : Entity
{
    /// <summary>
    ///     Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.Now;

    /// <summary>
    ///     Gets the Zone ID (Sensor number).
    /// </summary>
    public int ZoneId { get; }

    /// <summary>
    ///     Gets the sensor name.
    /// </summary>
    public string SensorName { get; } = null!;

    /// <summary>
    ///     Gets the type of this event.
    /// </summary>
    public ZoneMobilityEventType EventType { get; set; } = null!;

    [GeneratedRegex(@"^Zone:(\d*) ([a-zA-Z0-9 _.-]*), ([a-zA-Z0-9 _.-]*) - Mobility$")]
    private static partial Regex ZoneMobilityRegex();

    public static ZoneMobilityEvent? ParseFrom(string message, out bool succeeded)
    {
        Match match = ZoneMobilityRegex().Match(message);

        succeeded = true;
        return new ZoneMobilityEvent();
    }
}