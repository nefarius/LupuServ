using Coravel.Invocable;

using LupuServ.Models.Web;
using LupuServ.Services.Web;

namespace LupuServ.Invocables;

/// <summary>
///     Scheduled job polling for sensor status.
/// </summary>
public class GetSensorListInvocable : IInvocable
{
    private readonly ILogger<GetSensorListInvocable> _logger;
    private readonly ISensorListApi _sensors;

    public GetSensorListInvocable(ILogger<GetSensorListInvocable> logger, ISensorListApi sensors)
    {
        _logger = logger;
        _sensors = sensors;
    }

    public async Task Invoke()
    {
        SensorListResponse status = await _sensors.GetSensorList();

        throw new NotImplementedException();
    }
}