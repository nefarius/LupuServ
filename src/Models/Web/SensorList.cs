using System.Text.Json.Serialization;

namespace LupuServ.Models.Web;

public sealed class SensorListResponse
{
    [JsonPropertyName("senrows")]
    public List<Senrow> Senrows { get; set; }
}

public sealed class Senrow
{
    [JsonPropertyName("no")]
    public string No { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("zone")]
    public string Zone { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("attr")]
    public string Attr { get; set; }

    [JsonPropertyName("cond")]
    public string Cond { get; set; }

    [JsonPropertyName("battery")]
    public string Battery { get; set; }

    [JsonPropertyName("tamp")]
    public string Tamp { get; set; }

    [JsonPropertyName("bypass")]
    public string Bypass { get; set; }
}