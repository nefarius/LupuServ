namespace LupuServ;

/// <summary>
///     The dynamic service configuration.
/// </summary>
public class ServiceConfig
{
    /// <summary>
    ///     The port the SMTP server will listen on.
    /// </summary>
    public int Port { get; set; } = 2025;
    
    /// <summary>
    ///     The <see href="https://www.cm.com/" /> API key.
    /// </summary>
    public string ApiKey { get; set; } = null!;

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