namespace HtmlCompiler.Core.Interfaces;

public interface IRenderingComponent
{
    /// <summary>
    /// the actual rendering logic
    /// </summary>
    /// <param name="content">the content to render</param>
    /// <returns>the rendered content</returns>
    public Task<string> RenderAsync(string content);
    
    /// <summary>
    /// default is true. If true, every loaded file is rendered by the rendering component. If false, then only the final main file is rendered by the rendering component.
    /// </summary>
    public bool PreRenderPartialFiles { get; }
}