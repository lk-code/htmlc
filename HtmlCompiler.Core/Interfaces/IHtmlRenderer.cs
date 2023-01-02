namespace HtmlCompiler.Core.Interfaces;

public interface IHtmlRenderer
{
    Task<string> RenderHtmlAsync(string sourceFullFilePath, string? cssOutputFilePath);
}
