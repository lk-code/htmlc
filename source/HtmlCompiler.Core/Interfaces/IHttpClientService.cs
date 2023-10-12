namespace HtmlCompiler.Core.Interfaces;

public interface IHttpClientService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    Task<string> GetAsync(Uri uri);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="targetFilePath"></param>
    /// <param name="progressCallback"></param>
    /// <returns></returns>
    Task DownloadFileAsync(Uri uri, string targetFilePath, Action<long, long> progressCallback);
}