using System.Text.RegularExpressions;

namespace LupuServ.Util;

/// <summary>
///     Decodes a status message received from Alarm Centre.
/// </summary>
internal sealed class MessagePacket
{
    private static readonly Regex Pattern = new(@"Zone\:(\d) (.*), (.*)");

    private MessagePacket(int zoneId, string sensorName, string eventMessage)
    {
        ZoneId = zoneId;
        SensorName = sensorName;
        EventMessage = eventMessage;
    }

    /// <summary>
    ///     Gets the Zone ID (Sensor number).
    /// </summary>
    public int ZoneId { get; }

    /// <summary>
    ///     Gets the sensor name.
    /// </summary>
    public string SensorName { get; }

    /// <summary>
    ///     Gets the event message content.
    /// </summary>
    public string EventMessage { get; }

    /// <summary>
    ///     Decodes a received string message into individual message components.
    /// </summary>
    /// <param name="content">The source message text.</param>
    /// <returns>The decoded <see cref="MessagePacket" />.</returns>
    public static MessagePacket DecodeFrom(string content)
    {
        Match match = Pattern.Match(content);

        return new MessagePacket(
            int.Parse(match.Groups[1].Value),
            match.Groups[2].Value,
            match.Groups[3].Value
        );
    }
}