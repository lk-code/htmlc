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

    /// <inheritdoc />
    public async Task DownloadFileAsync(Uri uri, string targetFilePath, Action<long, long> progressCallback)
    {
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        await using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();

        long totalBytes = response.Content.Headers.ContentLength ?? -1;
        long bytesRead = 0;
        byte[] buffer = new byte[8192];
        int read;

        using Stream streamToWriteTo = File.Open(targetFilePath, FileMode.Create);
        while ((read = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await streamToWriteTo.WriteAsync(buffer, 0, read);
            bytesRead += read;

            // Fortschritt mithilfe des Callbacks melden
            progressCallback?.Invoke(bytesRead, totalBytes);
        }
    }
}