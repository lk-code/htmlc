namespace HtmlCompiler.Core.Interfaces;

public interface IRenderingComponent
{
    /// <summary>
    /// the actual rendering logic
    /// </summary>
    /// <param name="content">the content to render</param>
    /// <returns>the rendered content</returns>
    public Task<string> RenderAsync(string content);
}