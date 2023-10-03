using System.Diagnostics.CodeAnalysis;

namespace LupuServ;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class CMConfig
{
    /// <summary>
    ///     The <see href="https://www.cm.com/" /> API key.
    /// </summary>
    public string ApiKey { get; set; } = null!;
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class ClickSendConfig
{
    public string? Username { get; set; }
    
    public string? Token { get; set; }
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
    
    [Obsolete("Use CM.ApiKey instead")]
    public string ApiKey { get; set; } = null!;

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
}