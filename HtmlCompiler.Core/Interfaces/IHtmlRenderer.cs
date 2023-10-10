using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core.Interfaces;

public interface IHtmlRenderer
{
    /// <summary>
    /// returns the logging instance for renderer access
    /// </summary>
    ILogger<IHtmlRenderer> Logger { get; }
    /// <summary>
    /// 
    /// </summary>
    public IFileSystemService FileSystemService { get; }
    /// <summary>
    /// 
    /// </summary>
    public IDateTimeProvider DateTimeProvider { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceFullFilePath"></param>
    /// <param name="sourceDirectory"></param>
    /// <param name="outputDirectory"></param>
    /// <param name="cssOutputFilePath"></param>
    /// <param name="globalVariables"></param>
    /// <param name="callLevel"></param>
    /// <returns></returns>
    Task<string> RenderHtmlFromFileAsync(string sourceFullFilePath,
        string sourceDirectory,
        string outputDirectory,
        string? cssOutputFilePath,
        JsonElement? globalVariables,
        long callLevel);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlString"></param>
    /// <param name="sourceFullFilePath"></param>
    /// <param name="sourceDirectory"></param>
    /// <param name="outputDirectory"></param>
    /// <param name="cssOutputFilePath"></param>
    /// <param name="globalVariables"></param>
    /// <param name="callLevel"></param>
    /// <returns></returns>
    Task<string> RenderHtmlStringAsync(string htmlString,
        string sourceFullFilePath,
        string sourceDirectory,
        string outputDirectory,
        string? cssOutputFilePath,
        JsonElement? globalVariables,
        long callLevel);
}