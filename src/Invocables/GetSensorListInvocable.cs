using System.Text;

using Coravel.Invocable;

using LupuServ.Models.Web;
using LupuServ.Services.Web;

using Microsoft.Extensions.Options;

namespace LupuServ.Invocables;

/// <summary>
///     Scheduled job polling for sensor status.
/// </summary>
public class GetSensorListInvocable : IInvocable
{
    private readonly IOptions<ServiceConfig> _config;
    private readonly IGotifySensorsApi? _gotifySensorsApi;
    private readonly ILogger<GetSensorListInvocable> _logger;
    private readonly ISensorListApi _sensors;

    public GetSensorListInvocable(ILogger<GetSensorListInvocable> logger, ISensorListApi sensors,
        IOptions<ServiceConfig> config,
        IGotifySensorsApi? gotifySensorsApi = null)
    {
        _logger = logger;
        _sensors = sensors;
        _config = config;
        _gotifySensorsApi = gotifySensorsApi;
    }

    public async Task Invoke()
    {
        try
        {
            _logger.LogInformation("Fetching sensor status");

            SensorListResponse status = await _sensors.GetSensorList();

            _logger.LogInformation("Got sensor status");

            StringBuilder sb = new();

            sb.AppendLine("""
                          | Sensor | Status |
                          | --- | --- |
                          """);

            foreach (Senrow senrow in status.Senrows)
            {
                _logger.LogInformation("Sensor result: {Sensor}", senrow);

                sb.AppendLine(
                    $"| {senrow.ToString()} | Battery: {senrow.Battery}, Bypass: {senrow.Bypass}, Tampering: {senrow.Tamp} |");
            }

            await _gotifySensorsApi.SendMessage(
                _config.Value,
                sb.ToString()
            );

            _logger.LogInformation("Done fetching sensor status");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failure during sensor status retrieval");
        }
    }
}