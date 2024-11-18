#nullable disable
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LupuServ.Models.Web;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class SmsRequestMessage
{
    [JsonPropertyName("to")]
    public string To { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class SmsRequest
{
    [JsonPropertyName("messages")]
    public List<SmsRequestMessage> Messages { get; set; } = new();
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class SmsResponseCurrency
{
    [JsonPropertyName("currency_name_short")]
    public string CurrencyNameShort { get; set; }

    [JsonPropertyName("currency_prefix_d")]
    public string CurrencyPrefixD { get; set; }

    [JsonPropertyName("currency_prefix_c")]
    public string CurrencyPrefixC { get; set; }

    [JsonPropertyName("currency_name_long")]
    public string CurrencyNameLong { get; set; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class SmsResponseData
{
    [JsonPropertyName("total_price")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("queued_count")]
    public int QueuedCount { get; set; }

    [JsonPropertyName("messages")]
    public List<SmsResponseMessage> Messages { get; set; }

    [JsonPropertyName("_currency")]
    public SmsResponseCurrency Currency { get; set; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class SmsResponseMessage
{
    [JsonPropertyName("direction")]
    public string Direction { get; set; }

    [JsonPropertyName("date")]
    public int Date { get; set; }

    [JsonPropertyName("to")]
    public string To { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }

    [JsonPropertyName("from")]
    public string From { get; set; }

    [JsonPropertyName("schedule")]
    public int Schedule { get; set; }

    [JsonPropertyName("message_id")]
    public string MessageId { get; set; }

    [JsonPropertyName("message_parts")]
    public int MessageParts { get; set; }

    [JsonPropertyName("message_price")]
    public string MessagePrice { get; set; }

    [JsonPropertyName("custom_string")]
    public string CustomString { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("subaccount_id")]
    public int SubaccountId { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("carrier")]
    public string Carrier { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class SmsResponse
{
    [JsonPropertyName("http_code")]
    public int HttpCode { get; set; }

    [JsonPropertyName("response_code")]
    public string ResponseCode { get; set; }

    [JsonPropertyName("response_msg")]
    public string ResponseMsg { get; set; }

    [JsonPropertyName("data")]
    public SmsResponseData Data { get; set; }
}