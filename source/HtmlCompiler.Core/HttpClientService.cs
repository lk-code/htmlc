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

    public async Task DownloadFileAsync(Uri uri, string targetFilePath)
    {
        using HttpClient httpClient = new HttpClient();

        using HttpResponseMessage response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}");
        }

        await using FileStream fileStream = File.Create(targetFilePath);
        
        await response.Content.CopyToAsync(fileStream);
    }
}