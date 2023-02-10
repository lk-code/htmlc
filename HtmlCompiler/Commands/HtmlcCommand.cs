using Cocona;
using HtmlCompiler.Core.Interfaces;
using Microsoft.ClearScript;

namespace HtmlCompiler.Commands;

public class HtmlcCommand
{
    private readonly IHtmlWatcher _htmlWatcher;
    private readonly IProjectManager _projectManager;

    public HtmlcCommand(IHtmlWatcher htmlWatcher,
        IProjectManager projectManager)
    {
        this._htmlWatcher = htmlWatcher ?? throw new ArgumentNullException(nameof(htmlWatcher));
        this._projectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
    }

    [Command("new")]
    public async Task New([Option('v', Description = "added Visual Studio Code settings file")] bool? vscode = null,
        [Option('l', Description = "add configuration for VSCode Extension LiveServer (see documentation)")] bool? vsliveserver = null,
        [Option('d', Description = "creates a simple Dockerfile with nginx configuration")] bool? docker = null)
    {
        // create new project
        string projectPath = Directory.GetCurrentDirectory();
        string sourcePath = Path.Combine(projectPath, "src");
        string outputPath = Path.Combine(projectPath, "dist");
        
        await this._projectManager.CreateProjectAsync(projectPath);
        
        // add vscode if requested
        if (vscode == true)
        {
            await this._projectManager.AddVSCodeSupportAsync(projectPath);
        }
        
        // add vscode liveserver if requested
        if (vsliveserver == true)
        {
            await this._projectManager.AddVSCodeLiveServerConfigurationAsync(projectPath);
        }
        
        // add docker if requested
        if (docker == true)
        {
            await this._projectManager.AddDockerSupportAsync(sourcePath);
        }
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