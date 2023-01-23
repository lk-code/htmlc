using System;
namespace HtmlCompiler.Core.Interfaces;

public interface IStyleCompiler
{
    Task<string?> CompileStyleAsync(string sourceDirectoryPath, string outputDirectoryPath, string? styleSourceFilePath);
    Task<string> GetStyleContent(string sourceDirectoryPath, string sourceFilePath);
}