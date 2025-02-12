namespace LupuServ.Services.Interfaces;

/// <summary>
///     Defines a possible SMS gateway implementation.
/// </summary>
public interface IMessageGateway
{
    Task<bool> SendMessage(string body, string from, IEnumerable<string> recipients, CancellationToken token = default);
}