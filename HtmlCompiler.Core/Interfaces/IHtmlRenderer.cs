namespace HtmlCompiler.Core.Interfaces;

public interface IHtmlRenderer
{
    Task RenderToFileAsync(string sourceFile, string outputFile);
}
