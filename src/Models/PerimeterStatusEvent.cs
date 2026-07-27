using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LupuServ.Models;

/// <summary>
///     Represents a perimeter status event.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed partial class PerimeterStatusEvent
{
    /// <summary>
    ///     Creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    ///     Gets the username that initiated the event.
    /// </summary>
    public string Username { get; private set; } = null!;

    /// <summary>
    ///     Gets the type of this event.
    /// </summary>
    public PerimeterStatusEventType EventType { get; private set; } = null!;

    [GeneratedRegex(@"^([a-zA-Z\u00F0-\u02AF0-9 _.-]*), ([Arm|Home|Disarm]*)$")]
    private static partial Regex PerimeterStatusRegex();

    /// <summary>
    ///     Maps this event to a flat store row.
    /// </summary>
    public EventRecord ToRecord(string? rawMessage) =>
        new(CreatedAt, "PerimeterStatus", EventType.Name, EventType.Value, null, null, Username, rawMessage);

    public static bool TryParse(string message, out PerimeterStatusEvent? parsedEvent)
    {
        Match match = PerimeterStatusRegex().Match(message);

        if (!match.Success)
        {
            parsedEvent = null;
            return false;
        }

        string userName = match.Groups[1].Value;

        if (!PerimeterStatusEventType.TryFromName(match.Groups[2].Value, out PerimeterStatusEventType? eventType))
        {
            parsedEvent = null;
            return false;
        }

        parsedEvent = new PerimeterStatusEvent
        {
            CreatedAt = DateTimeOffset.UtcNow, Username = userName, EventType = eventType
        };

        return true;
    }
}
