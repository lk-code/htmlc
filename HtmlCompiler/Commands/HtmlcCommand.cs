using System.Diagnostics;
using Cocona;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Commands;

public class HtmlcCommand
{
    private readonly IHtmlRenderer _htmlRenderer;
    private readonly IHtmlWatcher _htmlWatcher;

    public HtmlcCommand(IHtmlRenderer htmlRenderer,
        IHtmlWatcher htmlWatcher)
    {
        this._htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
        this._htmlWatcher = htmlWatcher ?? throw new ArgumentNullException(nameof(htmlWatcher));
    }

    [Command("compile")]
    public async Task Compile([Argument(Description = "path to the source file")] string sourceFile,
        [Argument(Description = "path to the output file")] string outputFile)
    {
        string fullOutputFilePath = outputFile.FromSourceFilePath(sourceFile);

        Console.WriteLine($"compile file {sourceFile} to {fullOutputFilePath}");

        try
        {
            await this._htmlRenderer.RenderToFileAsync(sourceFile, fullOutputFilePath);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"file {sourceFile} not found");
        }
    }

    [Command("watch")]
    public async Task Watch([Argument(Description = "path to the source files. If empty, then the /src is used in the current directory for scanning")] string? sourcePath = null,
        [Argument(Description = "path for the output. if empty, then the /dist folder is used in the current directory")] string? outputPath = null,
        [Option('s', Description = "path to the style file to compile (scss or less)")] string? style = null)
    {
        await this._htmlWatcher.WatchDirectoryAsync(sourcePath, outputPath, style);
    }
}