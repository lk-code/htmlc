using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public class BuildDateRenderer : RenderingBase
{
    public const string BUILDDATE_TAG = "@BuildDate";

    public BuildDateRenderer(RenderingConfiguration configuration,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        DateTime now = this._htmlRenderer.DateTimeProvider.Now();
        
        return content;
    }
}