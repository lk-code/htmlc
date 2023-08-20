using HtmlCompiler.Core.Models;

namespace HtmlCompiler.Core.Interfaces;

public interface IStyleRenderer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputContent"></param>
    /// <param name="styleSourceFilePath"></param>
    /// <param name="styleOutputFilePath"></param>
    /// <returns></returns>
    Task CompileStyle(string inputContent, string styleSourceFilePath, string styleOutputFilePath);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputContent"></param>
    /// <returns></returns>
    Task<StyleRenderingResult> Compile(string inputContent);
}