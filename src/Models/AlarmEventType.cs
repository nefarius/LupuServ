using System.Diagnostics.CodeAnalysis;

using Ardalis.SmartEnum;

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