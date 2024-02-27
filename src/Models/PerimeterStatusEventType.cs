using System.Diagnostics.CodeAnalysis;

using Ardalis.SmartEnum;

namespace LupuServ.Models;

/// <summary>
///     A type of <see cref="PerimeterStatusEvent"/>.
/// </summary>
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