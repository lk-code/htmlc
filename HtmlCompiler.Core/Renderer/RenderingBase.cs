using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core.Renderer;

public abstract class RenderingBase : IRenderingComponent
{
    private readonly ILogger<IRenderingComponent> _logger;
    protected readonly RenderingConfiguration _configuration;
    protected readonly IFileSystemService _fileSystemService;
    protected readonly IHtmlRenderer _htmlRenderer;

    /// <inheritdoc />
    public abstract Task<string> RenderAsync(string content);

    public virtual bool PreRenderPartialFiles { get; } = true;

    protected RenderingBase(ILogger<IRenderingComponent> logger,
        RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        _htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
    }
}