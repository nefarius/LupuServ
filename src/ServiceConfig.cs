using System.Diagnostics.CodeAnalysis;

namespace LupuServ;

/// <summary>
///     CM-API-specific settings.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class CMConfig
{
    /// <summary>
    ///     The <see href="https://www.cm.com/" /> API key.
    /// </summary>
    public Guid ApiKey { get; set; }
}

/// <summary>
///     ClickSend-API-specific settings.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class ClickSendConfig
{
    public string? Username { get; set; }

    public string? Token { get; set; }
}

/// <summary>
///     The supported SMS gateways.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum GatewayService
{
    CM,
    ClickSend
}

/// <summary>
///     Central Station Web Interface login data.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class CentralStationConfig
{
    /// <summary>
    ///     Web Interface base URL.
    /// </summary>
    public required Uri Address { get; set; }

    /// <summary>
    ///     Admin login username.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    ///     Admin login password.
    /// </summary>
    public required string Password { get; set; }
}

/// <summary>
///     Message-category-specific Gotify settings.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class GotifyInstanceConfig
{
    /// <summary>
    ///     Optional Gotify server URL, overriding <see cref="GotifyConfig.Url"/>.
    /// </summary>
    public Uri? Url { get; set; }
    
    /// <summary>
    ///     The application token to the message channel.
    /// </summary>
    public required string AppToken { get; set; }

    /// <summary>
    ///     The title of the message.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    ///     Optional message priority.
    /// </summary>
    public int Priority { get; set; } = 1;

    /// <summary>
    ///     Whether the channel will receive any messages.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    ///     Optional regular expression to filter out messages with matching content.
    /// </summary>
    public string? ExclusionPattern { get; set; }
}

/// <summary>
///     Optional Gotify integration config.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class GotifyConfig
{
    /// <summary>
    ///     Gotify server instance URL.
    /// </summary>
    public required Uri Url { get; set; }
    
    /// <summary>
    ///     Status message receiver settings.
    /// </summary>
    public GotifyInstanceConfig? Status { get; set; }

    /// <summary>
    ///     Alarm message receiver settings.
    /// </summary>
    public GotifyInstanceConfig? Alarm { get; set; }
    
    /// <summary>
    ///     System message receiver settings.
    /// </summary>
    public GotifyInstanceConfig? System { get; set; }
    
    /// <summary>
    ///     Sensor status receiver settings.
    /// </summary>
    public GotifyInstanceConfig? Sensors { get; set; }
}

/// <summary>
///     The dynamic service configuration.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class ServiceConfig
{
    /// <summary>
    ///     The port the SMTP server will listen on.
    /// </summary>
    public required int Port { get; set; } = 2025;

    /// <summary>
    ///     The preferred message sending gateway to use.
    /// </summary>
    public required GatewayService Gateway { get; set; } = GatewayService.ClickSend;

    /// <summary>
    ///     CM gateway details.
    /// </summary>
    public CMConfig? CM { get; set; }

    /// <summary>
    ///     ClickSend gateway details.
    /// </summary>
    public ClickSendConfig ClickSend { get; set; } = new();

    /// <summary>
    ///     The user part of the sender address sending status messages.
    /// </summary>
    public required string StatusUser { get; set; } = "status";

    /// <summary>
    ///     The user part of the sender address sending alarm messages.
    /// </summary>
    public required string AlarmUser { get; set; } = "alarm";

    /// <summary>
    ///     The "Mail From" field content.
    /// </summary>
    public required string From { get; set; } = "Lupusec";

    /// <summary>
    ///     One or more phone numbers to relay messages to.
    /// </summary>
    public List<string> Recipients { get; set; } = new();

    /// <summary>
    ///     Database name.
    /// </summary>
    public required string DatabaseName { get; set; } = "lupuserv-events";

    /// <summary>
    ///     Connection properties for the central station web interface.
    /// </summary>
    public required CentralStationConfig CentralStation { get; set; }

    /// <summary>
    ///     Optional Gotify integration.
    /// </summary>
    public GotifyConfig? Gotify { get; set; }
}