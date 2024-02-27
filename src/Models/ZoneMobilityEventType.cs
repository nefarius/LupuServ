using System.Diagnostics.CodeAnalysis;

using Ardalis.SmartEnum;

namespace LupuServ.Models;

/// <summary>
///     A type of <see cref="ZoneMobilityEvent"/>.
/// </summary>
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