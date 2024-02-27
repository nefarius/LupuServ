using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using MongoDB.Entities;

namespace LupuServ.Models;

/// <summary>
///     Represents a special sensor status event.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed partial class SensorStatusEvent : Entity
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
    public SensorStatusEventType EventType { get; private set; } = null!;

    [GeneratedRegex(@"^Zone:(\d*) ([a-zA-Z\u00F0-\u02AF0-9 _.-]*), ([a-zA-Z\u00F0-\u02AF0-9 _.-]*)$")]
    private static partial Regex SensorStatusRegex();

    public static bool TryParse(string message, out SensorStatusEvent? parsedEvent)
    {
        Match match = SensorStatusRegex().Match(message);

        if (!match.Success)
        {
            parsedEvent = null;
            return false;
        }

        int zoneId = int.Parse(match.Groups[1].Value);
        string sensorName = match.Groups[2].Value;

        if (!SensorStatusEventType.TryFromName(match.Groups[3].Value, out SensorStatusEventType? eventType))
        {
            parsedEvent = null;
            return false;
        }

        parsedEvent = new SensorStatusEvent
        {
            CreatedAt = DateTime.Now, ZoneId = zoneId, SensorName = sensorName, EventType = eventType
        };

        return true;
    }
}