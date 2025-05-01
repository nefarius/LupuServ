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

        if (!config.Gotify!.Status!.IsEnabled)
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

        if (!config.Gotify!.Sensors!.IsEnabled)
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

        if (!config.Gotify!.Alarm!.IsEnabled)
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

        if (!config.Gotify!.System!.IsEnabled)
        {
            return;
        }

        await api.CreateMessage(new GotifyMessage
        {
            Title = title, Message = message, Priority = config.Gotify!.System!.Priority
        });
    }
}