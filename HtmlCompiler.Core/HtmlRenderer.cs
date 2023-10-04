using System.Text.Json;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core;

public class HtmlRenderer : IHtmlRenderer
{
    private readonly Dictionary<int, Type> _renderingComponents = new()
    {
        { 100, typeof(LayoutRenderer) },
        { 200, typeof(FileTagRenderer) },
        { 300, typeof(MarkdownFileTagRenderer) },
        { 400, typeof(VariablesRenderer) },
        { 500, typeof(CommentTagRenderer) },
        { 600, typeof(HtmlEscapeBlockRenderer) },
        { 700, typeof(GlobalTagRenderer) },
        { 800, typeof(StylePathRenderer) },
        { 900, typeof(PageTitleRenderer) },
        { 2000, typeof(MetaTagRenderer) }
    };

    private readonly ILogger<HtmlRenderer> _logger;
    private readonly IFileSystemService _fileSystemService;

    public HtmlRenderer(ILogger<HtmlRenderer> logger,
        IFileSystemService fileSystemService)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    /// <inheritdoc />
    public async Task<string> RenderHtmlStringAsync(string htmlString,
        string sourceFullFilePath,
        string sourceDirectory,
        string outputDirectory,
        string? cssOutputFilePath,
        JsonElement? globalVariables, long callLevel)
    {
        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = sourceFullFilePath.GetBaseDirectory(),
            SourceDirectory = sourceDirectory,
            OutputDirectory = outputDirectory,
            CssOutputFilePath = cssOutputFilePath!,
            SourceFullFilePath = sourceFullFilePath,
            GlobalVariables = globalVariables,
            CallLevel = (callLevel + 1)
        };

        IRenderingComponent[] renderingComponents = this._renderingComponents
            .OrderBy(x => x.Key)
            .Select(x => x.Value)
            .BuildRenderingComponents(
                configuration,
                this._fileSystemService,
                this)
            .ToArray();

        string masterOutput = htmlString;
        foreach (IRenderingComponent renderingComponent in renderingComponents
                     .Where(x => x.PreRenderPartialFiles == true))
        {
            masterOutput = await renderingComponent.RenderAsync(masterOutput);
        }

        if (callLevel == 0)
        {
            foreach (IRenderingComponent renderingComponent in renderingComponents
                         .Where(x => x.PreRenderPartialFiles == false))
            {
                masterOutput = await renderingComponent.RenderAsync(masterOutput);
            }
        }

        return masterOutput;
    }

    /// <inheritdoc />
    public async Task<string> RenderHtmlFromFileAsync(string sourceFullFilePath,
        string sourceDirectory,
        string outputDirectory,
        string? cssOutputFilePath,
        JsonElement? globalVariables,
        long callLevel = 0)
    {
        sourceFullFilePath = Path.GetFullPath(sourceFullFilePath);
        string originalContent = await this._fileSystemService.FileReadAllTextAsync(sourceFullFilePath);

        string masterOutput = await this.RenderHtmlStringAsync(
            originalContent,
            sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath,
            globalVariables,
            callLevel);
        
        return masterOutput;
    }

    public ILogger<IHtmlRenderer> Logger => this._logger;
}