
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

public interface IGotifyStatusApi : IGotifyApi;

public interface IGotifyAlarmApi : IGotifyApi;

public interface IGotifySystemApi : IGotifyApi;

public interface IGotifySensorsApi : IGotifyApi;