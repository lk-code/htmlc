using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class ProjectManager : IProjectManager
{
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
                subfolder.EnsurePath();
            }

            string fullFilePath = Path.Combine(projectPath, filePath);
            if (!string.IsNullOrEmpty(templateContent))
            {
                await File.WriteAllTextAsync(fullFilePath, templateContent);
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
            await File.WriteAllTextAsync(fullFilePath, templateContent);
        }
    }

    /// <inheritdoc/>
    public async Task AddVSCodeSupportAsync(string projectPath)
    {
        string filePath = ".vscode/settings.json";
        string? templateContent = await this.GetTemplateContentAsync("htmlc_vscode_settings_json");

        string fullFilePath = Path.Combine(projectPath, filePath);
        string vsDirectory = Path.GetDirectoryName(fullFilePath)!;
        vsDirectory.EnsurePath();

        if (!string.IsNullOrEmpty(templateContent))
        {
            await File.WriteAllTextAsync(fullFilePath, templateContent);
        }
    }

    /// <inheritdoc/>
    public async Task AddVSCodeLiveServerConfigurationAsync(string projectPath)
    {
        string filePath = ".vscode/settings.json";
        string fullFilePath = Path.Combine(projectPath, filePath);
        
        string fileContent = await File.ReadAllTextAsync(fullFilePath);

        fileContent = fileContent.UpdateJsonProperty("liveServer.settings.root", "/dist");
        
        await File.WriteAllTextAsync(fullFilePath, fileContent);
    }

    internal async Task<string?> GetTemplateContentAsync(string template)
    {
        string? content = null;

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"HtmlCompiler.Core.FileTemplates.{template}.template";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
            using (StreamReader reader = new StreamReader(stream))
            {
                content = await reader.ReadToEndAsync();
            }
        }
        catch (Exception)
        {
        }

        return content;
    }
}