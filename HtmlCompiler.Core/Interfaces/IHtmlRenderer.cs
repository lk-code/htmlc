using System.Text.Json;

namespace HtmlCompiler.Core.Interfaces;

public interface IHtmlRenderer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceFullFilePath"></param>
    /// <param name="sourceDirectory"></param>
    /// <param name="outputDirectory"></param>
    /// <param name="cssOutputFilePath"></param>
    /// <param name="callLevel"></param>
    /// <returns></returns>
    Task<string> RenderHtmlAsync(string sourceFullFilePath,
        string sourceDirectory,
        string outputDirectory,
        string? cssOutputFilePath,
        JsonElement? globalVariables,
        long callLevel);
}