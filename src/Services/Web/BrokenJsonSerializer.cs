using System.Reflection;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Refit;

namespace LupuServ.Services.Web;

/// <summary>
///     Fixes the weird JSON-esc nonsense the central station webserver returns.
/// </summary>
internal sealed partial class BrokenJsonSerializer : IHttpContentSerializer
{
    public HttpContent ToHttpContent<T>(T item)
    {
        throw new NotImplementedException();
    }

    public async Task<T?> FromHttpContentAsync<T>(HttpContent content, CancellationToken cancellationToken = new())
    {
        string mangledBody = await content.ReadAsStringAsync(cancellationToken);
        Match bodyMatch = JsonExtractorRegex().Match(mangledBody);
        string jsonBody = bodyMatch.Groups[1].Value;

        return JsonConvert.DeserializeObject<T>(jsonBody);
    }

    public string? GetFieldNameForProperty(PropertyInfo propertyInfo)
    {
        throw new NotImplementedException();
    }

    [GeneratedRegex(@"\/\*-secure-[\r\n](.*)\*\/", RegexOptions.Singleline)]
    private partial Regex JsonExtractorRegex();
}