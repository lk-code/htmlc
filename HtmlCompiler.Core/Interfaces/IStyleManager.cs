using System;
namespace HtmlCompiler.Core.Interfaces;

public interface IStyleManager
{
    Task<string?> CompileStyleAsync(string sourceDirectoryPath, string outputDirectoryPath, string? styleSourceFilePath);
    Task<string> GetStyleContent(string sourceDirectoryPath, string sourceFilePath);
}