using System.IO.Compression;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class ProjectManager : IProjectManager
{
    private readonly IFileSystemService _fileSystemService;
    private readonly IResourceLoader _resourceLoader;

    public ProjectManager(IFileSystemService fileSystemService,
        IResourceLoader resourceLoader)
    {
        this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        this._resourceLoader = resourceLoader ?? throw new ArgumentNullException(nameof(resourceLoader));
    }
    
    /// <inheritdoc/>
    public async Task CreateProjectAsync(string projectPath)
    {
        Dictionary<string, string> filesToCreate = new()
        {
            { ".gitignore", "htmlc_gitignore" },
            { "src/index.html", "htmlc_index_html" },
            { "src/shared/_layout.html", "htmlc_shared_layout_html" },
            { "dist", "" }
        };

        foreach (var fileToCreate in filesToCreate)
        {
            string filePath = fileToCreate.Key;
            string templateKey = fileToCreate.Value;
            string? templateContent = null;
            if (!string.IsNullOrEmpty(templateKey))
            {
                templateContent = await this.GetTemplateContentAsync(templateKey);
            }

            string? folderPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                string subfolder = Path.Combine(projectPath, folderPath);
                this._fileSystemService.EnsurePath(subfolder);
            }

            string fullFilePath = Path.Combine(projectPath, filePath);
            if (!string.IsNullOrEmpty(templateContent))
            {
                await this._fileSystemService.FileWriteAllTextAsync(fullFilePath, templateContent);
            }
        }
    }
        
    private async Task ExtractZipAsync(string zipFilePath, string extractPath)
    {
        using (FileStream zipStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
        {
            foreach (var entry in archive.Entries)
            {
                string entryFilePath = Path.Combine(extractPath, entry.FullName);
                string canonicalDestinationPath = Path.GetFullPath(entryFilePath);
                Directory.CreateDirectory(Path.GetDirectoryName(canonicalDestinationPath));

                if (!entry.FullName.EndsWith("/")) // Ignoriere Verzeichniseinträge
                {
                    using (var entryStream = entry.Open())
                    using (var targetStream = File.Create(canonicalDestinationPath))
                    {
                        await entryStream.CopyToAsync(targetStream);
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task AddDockerSupportAsync(string sourcePath)
    {
        string filePath = "Dockerfile";
        string? templateContent = await this.GetTemplateContentAsync("htmlc_dockerfile");
        
        string fullFilePath = Path.Combine(sourcePath, filePath);
        if (!string.IsNullOrEmpty(templateContent))
        {
            await this._fileSystemService.FileWriteAllTextAsync(fullFilePath, templateContent);
        }
    }

    /// <inheritdoc/>
    public async Task AddVSCodeSupportAsync(string projectPath)
    {
        string filePath = ".vscode/settings.json";
        string? templateContent = await this.GetTemplateContentAsync("htmlc_vscode_settings_json");

        string fullFilePath = Path.Combine(projectPath, filePath);
        string vsDirectory = Path.GetDirectoryName(fullFilePath)!;
        this._fileSystemService.EnsurePath(vsDirectory);

        if (!string.IsNullOrEmpty(templateContent))
        {
            await this._fileSystemService.FileWriteAllTextAsync(fullFilePath, templateContent);
        }
    }

    /// <inheritdoc/>
    public async Task AddVSCodeLiveServerConfigurationAsync(string projectPath)
    {
        string filePath = ".vscode/settings.json";
        string fullFilePath = Path.Combine(projectPath, filePath);
        
        string fileContent = await this._fileSystemService.FileReadAllTextAsync(fullFilePath);

        fileContent = fileContent.UpdateJsonProperty("liveServer.settings.root", "/dist");
        
        await this._fileSystemService.FileWriteAllTextAsync(fullFilePath, fileContent);
    }

    /// <inheritdoc/>
    public async Task AddTemplateAsync(string downloadedTemplatePath, string sourcePath)
    {
        // cleanup source directoy
        Directory.Delete(sourcePath, recursive: true);
        Directory.CreateDirectory(sourcePath);
        
        // load template
        await this.ExtractZipAsync(downloadedTemplatePath, sourcePath);
    }

    private async Task<string?> GetTemplateContentAsync(string template)
    {
        string resourceName = $"HtmlCompiler.Core.FileTemplates.{template}.template";
        string? content = await this._resourceLoader.GetResourceContentAsync(resourceName);
        
        return content;
    }
}