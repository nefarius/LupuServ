namespace LupuServ.Services;

public interface IMessageGateway
{
    Task<bool> SendMessage(string body, string from, IEnumerable<string> recipients, CancellationToken token = default);
}