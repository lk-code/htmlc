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
    Task DownloadFileAsync(Uri uri, string targetFilePath);
}