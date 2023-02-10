namespace HtmlCompiler.Core.Interfaces;

public interface IProjectManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectPath"></param>
    /// <returns></returns>
    Task CreateProjectAsync(string projectPath);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <returns></returns>
    Task AddDockerSupportAsync(string sourcePath);

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