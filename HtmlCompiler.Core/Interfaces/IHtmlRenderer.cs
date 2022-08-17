namespace HtmlCompiler.Core.Interfaces
{
    public interface IHtmlRenderer
    {
        Task<string> RenderAsync(string sourceFile, string outputFile);
    }
}
