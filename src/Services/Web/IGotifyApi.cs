
using LupuServ.Models.Web;

using Refit;
namespace LupuServ.Services.Web;

/// <summary>
///     Gotify REST API implementation.
/// </summary>
public interface IGotifyApi
{
    /// <summary>
    ///     Create a message.
    /// </summary>
    /// <remarks>https://gotify.net/api-docs#/message/createMessage</remarks>
    [Post("/message")]
    Task CreateMessage([Body] GotifyMessage message);
}

/// <summary>
///     Gotify instance for status messages.
/// </summary>
public interface IGotifyStatusApi : IGotifyApi;

/// <summary>
///     Gotify instance for alarm messages.
/// </summary>
public interface IGotifyAlarmApi : IGotifyApi;

/// <summary>
///     Gotify instance for system messages (e.g. exceptions).
/// </summary>
public interface IGotifySystemApi : IGotifyApi;

/// <summary>
///     Gotify instance for sensor status messages.
/// </summary>
public interface IGotifySensorsApi : IGotifyApi;