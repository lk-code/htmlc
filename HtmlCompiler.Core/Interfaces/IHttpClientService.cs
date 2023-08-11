namespace HtmlCompiler.Core.Interfaces;

public interface IHttpClientService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    Task<string> GetAsync(Uri uri);
}