using LupuServ.Models.Web;

using Refit;

namespace LupuServ.Services.Web;

public interface ISensorListApi
{
    [Get("/action/sensorListGet")]
    Task<SensorListResponse> GetSensorList();
}