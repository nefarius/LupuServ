#nullable disable
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace LupuServ.Models.Web;

/// <summary>
///     Central Station sensor list response.
/// </summary>
public sealed class SensorListResponse
{
    [JsonProperty("senrows")]
    public List<Senrow> Senrows { get; set; }
}

/// <summary>
///     Sensors with active bypass do not trigger an alarm, if tampering is detected.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum BypassState
{
    Aktiv,
    Inaktiv
}

/// <summary>
///     Sensor row.
/// </summary>
public sealed class Senrow
{
    [JsonProperty("no")]
    public int No { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("zone")]
    public string Zone { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("attr")]
    public string Attr { get; set; }

    [JsonProperty("cond")]
    public string Cond { get; set; }

    [JsonProperty("battery")]
    public string Battery { get; set; }

    [JsonProperty("tamp")]
    public string Tamp { get; set; }

    [JsonProperty("bypass")]
    public BypassState Bypass { get; set; }

    public override string ToString()
    {
        return $"Zone: {Zone}, {Name}";
    }
}