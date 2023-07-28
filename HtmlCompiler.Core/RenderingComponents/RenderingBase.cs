using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.RenderingComponents;

public abstract class RenderingBase : IRenderingComponent
{
    protected readonly RenderingConfiguration _configuration;
    protected readonly IFileSystemService _fileSystemService;
    protected readonly IHtmlRenderer _htmlRenderer;

    /// <inheritdoc />
    public abstract Task<string> RenderAsync(string content);

    protected RenderingBase(RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        _htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
    }
}