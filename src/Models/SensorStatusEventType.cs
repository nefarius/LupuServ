using System.Diagnostics.CodeAnalysis;

using Ardalis.SmartEnum;

namespace LupuServ.Models;

/// <summary>
///     A type of <see cref="SensorStatusEvent" />.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed class SensorStatusEventType : SmartEnum<SensorStatusEventType>
{
    public static readonly SensorStatusEventType PeriodicTestReport =
        new("Periodischer Test Report", 1) { IsCritical = false };

    public static readonly SensorStatusEventType SensorMonitoringError =
        new("Sensorüberwachungsfehler", 2) { IsCritical = true };

    private SensorStatusEventType(string name, int value) : base(name, value)
    {
    }

    /// <summary>
    ///     Gets whether the event should be considered critical (inform the user in some way about it).
    /// </summary>
    public bool IsCritical { get; init; }
}