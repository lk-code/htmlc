using System.Reflection;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class ProjectManager : IProjectManager
{
    public async Task CreateProjectAsync(string path)
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
                string subfolder = Path.Combine(path, folderPath);
                subfolder.EnsurePath();
            }

            string fullFilePath = Path.Combine(path, filePath);

            if (!string.IsNullOrEmpty(templateContent))
            {
                await File.WriteAllTextAsync(fullFilePath, templateContent);
            }
        }
    }

    public Task AddDockerSupportAsync(string projectPath)
    {
        throw new NotImplementedException();
    }

    public Task AddVSCodeSupportAsync(string projectPath)
    {
        throw new NotImplementedException();
    }

    public Task AddVSCodeLiveServerConfigurationAsync(string projectPath)
    {
        throw new NotImplementedException();
    }

    internal async Task<string?> GetTemplateContentAsync(string template)
    {
        string? content = null;

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"HtmlCompiler.Core.FileTemplates.{template}.template";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
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