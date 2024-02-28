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
///     The dynamic service configuration.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class ServiceConfig
{
    /// <summary>
    ///     The port the SMTP server will listen on.
    /// </summary>
    public int Port { get; set; } = 2025;

    /// <summary>
    ///     The preferred message sending gateway to use.
    /// </summary>
    public GatewayService Gateway { get; set; } = GatewayService.ClickSend;

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
    public string StatusUser { get; set; } = "status";

    /// <summary>
    ///     The user part of the sender address sending alarm messages.
    /// </summary>
    public string AlarmUser { get; set; } = "alarm";

    /// <summary>
    ///     The "Mail From" field content.
    /// </summary>
    public string From { get; set; } = "Lupusec";

    /// <summary>
    ///     One or more phone numbers to relay messages to.
    /// </summary>
    public List<string> Recipients { get; set; } = new();

    /// <summary>
    ///     Database name.
    /// </summary>
    public string DatabaseName { get; set; } = "lupuserv-events";

    /// <summary>
    ///     Connection properties for the central station web interface.
    /// </summary>
    public required CentralStationConfig CentralStation { get; set; }
}