using Coravel.Invocable;

using LupuServ.Models.Web;
using LupuServ.Services.Web;

namespace LupuServ.Invocables;

/// <summary>
///     Scheduled job polling for sensor status.
/// </summary>
public class GetSensorListInvocable : IInvocable
{
    private readonly IGotifySensorsApi? _gotifySensorsApi;
    private readonly ILogger<GetSensorListInvocable> _logger;
    private readonly ISensorListApi _sensors;

    public GetSensorListInvocable(ILogger<GetSensorListInvocable> logger, ISensorListApi sensors,
        IGotifySensorsApi? gotifySensorsApi = null)
    {
        _logger = logger;
        _sensors = sensors;
        _gotifySensorsApi = gotifySensorsApi;
    }

    public async Task Invoke()
    {
        SensorListResponse status = await _sensors.GetSensorList();

        foreach (Senrow senrow in status.Senrows)
        {
            _logger.LogInformation("Sensor result: {Sensor}", senrow);

            if (_gotifySensorsApi is not null)
            {
                await _gotifySensorsApi.CreateMessage(new GotifyMessage
                {
                    Title = senrow.ToString(),
                    Message = $"Battery: {senrow.Battery}, Bypass: {senrow.Bypass}, Tampering: {senrow.Tamp}"
                });
            }
        }
    }
}