namespace HtmlCompiler.Core.Interfaces;

public interface IProjectManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task CreateProjectAsync(string path);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectPath"></param>
    /// <returns></returns>
    Task AddDockerSupportAsync(string projectPath);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectPath"></param>
    /// <returns></returns>
    Task AddVSCodeSupportAsync(string projectPath);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectPath"></param>
    /// <returns></returns>
    Task AddVSCodeLiveServerConfigurationAsync(string projectPath);
}