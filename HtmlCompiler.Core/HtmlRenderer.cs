using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;

namespace HtmlCompiler.Core;

public class HtmlRenderer : IHtmlRenderer
{
    private readonly Dictionary<int, Type> _renderingComponents = new()
    {
        { 100, typeof(FileTagRenderer) },
        { 200, typeof(LayoutRenderer) },
        { 300, typeof(FileTagRenderer) },
        { 400, typeof(CommentTagRenderer) },
        { 500, typeof(HtmlEscapeBlockRenderer) },
        { 600, typeof(StylePathRenderer) },
        { 700, typeof(PageTitleRenderer) },
        { 800, typeof(MetaTagRenderer) }
    };

    private readonly IFileSystemService _fileSystemService;

    public HtmlRenderer(IFileSystemService fileSystemService)
    {
        this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    /// <inheritdoc />
    public async Task<string> RenderHtmlAsync(string sourceFullFilePath,
        string sourceDirectory,
        string outputDirectory,
        string? cssOutputFilePath)
    {
        sourceFullFilePath = Path.GetFullPath(sourceFullFilePath);
        string originalContent = await this._fileSystemService.FileReadAllTextAsync(sourceFullFilePath);

        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = sourceFullFilePath.GetBaseDirectory(),
            SourceDirectory = sourceDirectory,
            OutputDirectory = outputDirectory,
            CssOutputFilePath = cssOutputFilePath,
            SourceFullFilePath = sourceFullFilePath
        };
        
        IEnumerable<IRenderingComponent> renderingComponents = this._renderingComponents
            .OrderBy(x => x.Key)
            .Select(x => x.Value)
            .BuildRenderingComponents(
                configuration,
                this._fileSystemService,
                this);

        string masterOutput = originalContent;
        foreach (IRenderingComponent renderingComponent in renderingComponents)
        {
            masterOutput = await renderingComponent.RenderAsync(masterOutput);
        }
        
        return masterOutput;
    }
}