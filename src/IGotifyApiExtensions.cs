using System.Text.RegularExpressions;

using LupuServ.Models.Web;
using LupuServ.Services.Web;

namespace LupuServ;

internal static class GotifyApiExtensions
{
    public static async Task SendMessage(this IGotifyStatusApi? api, ServiceConfig config, string message)
    {
        if (api is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(config.Gotify!.Status!.ExclusionPattern) &&
            Regex.IsMatch(message, config.Gotify.Status!.ExclusionPattern))
        {
            return;
        }

        await api.CreateMessage(new GotifyMessage
        {
            Title = config.Gotify!.Status!.Title, Message = message, Priority = config.Gotify!.Status!.Priority
        });
    }

    public static async Task SendMessage(this IGotifySensorsApi? api, ServiceConfig config, string title,
        string message)
    {
        if (api is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(config.Gotify!.Sensors!.ExclusionPattern) &&
            Regex.IsMatch(message, config.Gotify.Sensors!.ExclusionPattern))
        {
            return;
        }

        await api.CreateMessage(new GotifyMessage
        {
            Title = title, Message = message, Priority = config.Gotify!.Sensors!.Priority
        });
    }

    public static async Task SendMessage(this IGotifyAlarmApi? api, ServiceConfig config, string message)
    {
        if (api is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(config.Gotify!.Alarm!.ExclusionPattern) &&
            Regex.IsMatch(message, config.Gotify.Alarm!.ExclusionPattern))
        {
            return;
        }

        await api.CreateMessage(new GotifyMessage
        {
            Title = config.Gotify!.Alarm!.Title, Message = message, Priority = config.Gotify!.Alarm!.Priority
        });
    }

    public static async Task SendMessage(this IGotifySystemApi? api, ServiceConfig config, string title,
        string message)
    {
        if (api is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(config.Gotify!.System!.ExclusionPattern) &&
            Regex.IsMatch(message, config.Gotify.System!.ExclusionPattern))
        {
            return;
        }

        await api.CreateMessage(new GotifyMessage
        {
            Title = title, Message = message, Priority = config.Gotify!.System!.Priority
        });
    }
}