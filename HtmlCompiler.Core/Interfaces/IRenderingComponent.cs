using HtmlCompiler.Core.RenderingComponents;

namespace HtmlCompiler.Core.Interfaces;

public interface IRenderingComponent
{
    /// <summary>
    /// defines the order in which the renderer will be executed
    /// </summary>
    public short Order { get; }
    /// <summary>
    /// the actual rendering logic
    /// </summary>
    /// <param name="content">the content to render</param>
    /// <returns>the rendered content</returns>
    public Task<string> RenderAsync(string content);
}