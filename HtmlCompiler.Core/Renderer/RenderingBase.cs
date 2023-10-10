using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public abstract class RenderingBase : IRenderingComponent
{
    protected readonly RenderingConfiguration _configuration;
    protected readonly IHtmlRenderer _htmlRenderer;
    
    public IFileSystemService FileSystemService => this._htmlRenderer.FileSystemService;
    public IDateTimeProvider DateTimeProvider => this._htmlRenderer.DateTimeProvider;

    /// <inheritdoc />
    public abstract Task<string> RenderAsync(string content);

    public virtual bool PreRenderPartialFiles { get; } = true;

    protected RenderingBase(RenderingConfiguration configuration,
        IHtmlRenderer htmlRenderer)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
    }
}