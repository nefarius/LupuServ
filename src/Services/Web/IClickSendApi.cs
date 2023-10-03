using LupuServ.Models.Web;

using Refit;

namespace LupuServ.Services.Web;

public interface IClickSendApi
{
    [Post("/v3/sms/send")]
    Task<SmsResponse> SendSms([Body] SmsRequest request);
}