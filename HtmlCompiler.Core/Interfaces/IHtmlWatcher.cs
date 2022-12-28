using System;
namespace HtmlCompiler.Core.Interfaces;

public interface IHtmlWatcher
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="outputPath"></param>
    /// <returns></returns>
    Task WatchDirectoryAsync(string? sourcePath, string? outputPath);
}