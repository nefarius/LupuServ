
using LupuServ.Models.Web;

using Refit;
namespace LupuServ.Services.Web;

/// <summary>
///     Gotify REST API implementation.
/// </summary>
internal interface IGotifyApi
{
    /// <summary>
    ///     Create a message.
    /// </summary>
    /// <remarks>https://gotify.net/api-docs#/message/createMessage</remarks>
    [Post("/message")]
    Task CreateMessage([Body] GotifyMessage message);
}

internal interface IGotifyStatusApi : IGotifyApi;

internal interface IGotifyAlarmApi : IGotifyApi;

internal interface IGotifySystemApi : IGotifyApi;