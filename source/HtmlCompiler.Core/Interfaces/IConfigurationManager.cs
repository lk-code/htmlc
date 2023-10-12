namespace HtmlCompiler.Config;

/// <summary>
/// 
/// </summary>
public interface IConfigurationManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task AddAsync(string key, string value);

    /// <summary>
    /// /
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task RemoveAsync(string key, string value);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task SetAsync(string key, string value);
}