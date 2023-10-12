namespace HtmlCompiler.Core.Interfaces;

public interface IStyleManager
{
    Task<string?> CompileStyleAsync(string sourceDirectoryPath, string outputDirectoryPath, string? styleSourceFilePath);
}