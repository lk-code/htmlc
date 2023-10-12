using System;
namespace HtmlCompiler.Core.Interfaces;

public interface IFileWatcher
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="outputPath"></param>
    /// <param name="styleFilePath"></param>
    /// <returns></returns>
    Task WatchDirectoryAsync(string? sourcePath, string? outputPath, string? styleFilePath, bool watchDirectory = true);
}