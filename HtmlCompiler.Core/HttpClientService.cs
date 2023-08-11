using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class HttpClientService : IHttpClientService
{
    public HttpClientService()
    {
    }

    /// <inheritdoc />
    public async Task<string> GetAsync(Uri uri)
    {
        using HttpClient httpClient = new HttpClient();

        HttpResponseMessage response = await httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}