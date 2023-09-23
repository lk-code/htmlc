using Cocona;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Commands;

public class HtmlcCommand
{
    private readonly ILogger<HtmlcCommand> _logger;
    private readonly IFileWatcher _fileWatcher;
    private readonly IProjectManager _projectManager;
    private readonly ITemplateManager _templateManager;

    public HtmlcCommand(ILogger<HtmlcCommand> logger,
        IFileWatcher fileWatcher,
        IProjectManager projectManager,
        ITemplateManager templateManager)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._fileWatcher = fileWatcher ?? throw new ArgumentNullException(nameof(fileWatcher));
        this._projectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
        this._templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
    }

    [Command("new")]
    public async Task New([Option('v', Description = "added Visual Studio Code settings file")] bool? vscode = null,
        [Option('l', Description = "add configuration for VSCode Extension LiveServer (see documentation)")] bool? vsliveserver = null,
        [Option('d', Description = "creates a simple Dockerfile with nginx configuration")] bool? docker = null,
        [Option('t', Description = "creates a project based on the given template name")] string? template = null)
    {
        string? downloadedTemplatePath = null;
        if (template is not null)
        {
            // search for template
            List<Template> templates = (await this._templateManager.SearchTemplatesAsync(template)).ToList();
            if (!templates.Any())
            {
                this._logger.LogError("No templates found.");
                
                return;
            }
            else if (templates.Count() > 1)
            {
                this._logger.LogError("Multiple templates found. Please specify the full template name (with repository url).");
                
                return;
            }
            
            // load template
            this._logger.LogInformation($"Download template '{templates.First().Name}'");
            downloadedTemplatePath = await this._templateManager.DownloadTemplateAsync(templates.First());
        }

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

        // add template files
        if (downloadedTemplatePath is not null)
        {
            await this._projectManager.AddTemplateAsync(downloadedTemplatePath, sourcePath);
        }
    }

    [Command("compile")]
    public async Task Compile(
        [Argument(Description = "path to the source files. If empty, then the /src is used in the current directory for scanning")]
        string? sourcePath = null,
        [Argument(Description = "path for the output. if empty, then the /dist folder is used in the current directory")]
        string? outputPath = null,
        [Option('s', Description = "path to the style file to compile (scss or sass)")]
        string? style = null)
    {
        await this._fileWatcher.WatchDirectoryAsync(sourcePath, outputPath, style, false);
    }

    [Command("watch")]
    public async Task Watch(
        [Argument(Description = "path to the source files. If empty, then the /src is used in the current directory for scanning")]
        string? sourcePath = null,
        [Argument(Description = "path for the output. if empty, then the /dist folder is used in the current directory")]
        string? outputPath = null,
        [Option('s', Description = "path to the style file to compile (scss or sass)")]
        string? style = null)
    {
        await this._fileWatcher.WatchDirectoryAsync(sourcePath, outputPath, style);
    }
}