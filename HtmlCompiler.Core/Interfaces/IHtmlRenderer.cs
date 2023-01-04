namespace HtmlCompiler.Core.Interfaces;

public interface IHtmlRenderer
{
    Task<string> RenderHtmlAsync(string sourceFullFilePath, string sourceDirectory, string outputDirectory, string? cssOutputFilePath);
}
