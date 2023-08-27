using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public class GlobalTagRenderer : RenderingBase
{
    public GlobalTagRenderer(RenderingConfiguration configuration, IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer) : base(configuration, fileSystemService, htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        await Task.CompletedTask;
        
        return content;
    }
}