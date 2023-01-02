using System.Diagnostics;
using Cocona;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Commands;

public class HtmlcCommand
{
    private readonly IHtmlRenderer _htmlRenderer;
    private readonly IHtmlWatcher _htmlWatcher;
    private readonly IStyleCompiler _styleCompiler;

    public HtmlcCommand(IHtmlRenderer htmlRenderer,
        IHtmlWatcher htmlWatcher,
        IStyleCompiler styleCompiler)
    {
        this._htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
        this._htmlWatcher = htmlWatcher ?? throw new ArgumentNullException(nameof(htmlWatcher));
        this._styleCompiler = styleCompiler ?? throw new ArgumentNullException(nameof(styleCompiler));
    }

    [Command("compile")]
    public async Task Compile([Argument(Description = "path to the source files. If empty, then the /src is used in the current directory for scanning")] string? sourcePath = null,
        [Argument(Description = "path for the output. if empty, then the /dist folder is used in the current directory")] string? outputPath = null,
        [Option('s', Description = "path to the style file to compile (scss or sass)")] string? style = null)
    {
        await this._htmlWatcher.WatchDirectoryAsync(sourcePath, outputPath, style, false);
    }

    [Command("watch")]
    public async Task Watch([Argument(Description = "path to the source files. If empty, then the /src is used in the current directory for scanning")] string? sourcePath = null,
        [Argument(Description = "path for the output. if empty, then the /dist folder is used in the current directory")] string? outputPath = null,
        [Option('s', Description = "path to the style file to compile (scss or sass)")] string? style = null)
    {
        await this._htmlWatcher.WatchDirectoryAsync(sourcePath, outputPath, style);
    }
}