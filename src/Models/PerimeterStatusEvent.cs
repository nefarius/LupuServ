using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Ardalis.SmartEnum;

using MongoDB.Entities;

namespace LupuServ.Models;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed class PerimeterStatusEventType : SmartEnum<PerimeterStatusEventType>
{
    public static readonly PerimeterStatusEventType Arm = new("Arm", 1);
    public static readonly PerimeterStatusEventType Home = new("Home", 2);
    public static readonly PerimeterStatusEventType Disarm = new("Disarm", 3);

    private PerimeterStatusEventType(string name, int value) : base(name, value)
    {
    }
}

/// <summary>
///     Represents a perimeter status event.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed partial class PerimeterStatusEvent : Entity
{
    /// <summary>
    ///     Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

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

        parsedEvent = new PerimeterStatusEvent { CreatedAt = DateTime.Now, Username = userName, EventType = eventType };

        return true;
    }
}