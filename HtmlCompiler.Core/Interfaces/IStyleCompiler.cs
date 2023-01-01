using System;
namespace HtmlCompiler.Core.Interfaces;

public interface IStyleCompiler
{
    Task CompileStyleAsync(string sourceDirectoryPath, string outputDirectoryPath, string? styleSourceFilePath);
}