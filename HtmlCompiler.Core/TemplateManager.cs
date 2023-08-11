using System.Text.Json;
using HtmlCompiler.Config;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Core;

public class TemplateManager : ITemplateManager
{
    public const string DEFAULT_TEMPLATE_REPOSITORY = "https://github.com/lk-code/htmlc-templates/raw/main/";

    private readonly IConfiguration _configuration;
    private readonly IConfigurationManager _configurationManager;
    private readonly IHttpClientService _httpClientService;

    public TemplateManager(IConfiguration configuration,
        IConfigurationManager configurationManager,
        IHttpClientService httpClientService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
        _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Template>> SearchTemplatesAsync(string templateName)
    {
        List<string>? repositories = this._configuration.GetSection("template-repositories").Get<List<string>>();

        // ensure repositories is not null
        if (repositories is null)
        {
            repositories = new List<string>();
        }

        // add default template repository if not set
        await this.EnsureDefaultRepository(repositories);

        // load all index-files
        // hostname => index-content
        Dictionary<string, List<TemplateIndexEntry>> indexContents = new Dictionary<string, List<TemplateIndexEntry>>();
        foreach (string repository in repositories)
        {
            Uri repositoryUri = new Uri($"{repository}index.json");
            string content = await this._httpClientService.GetAsync(repositoryUri);
            TemplateIndex templateIndex = JsonSerializer.Deserialize<TemplateIndex>(content);

            indexContents.Add(repository, templateIndex.Templates);
        }

        IEnumerable<Template> templates = indexContents
            .SelectMany(indexContentEntry => indexContentEntry.Value.Select(entry =>
                new Template(entry.Name, entry.File, $"{indexContentEntry.Key}{entry.File}")
            ));

        // search for template via name
        IEnumerable<Template> searchNameResults = templates.Where(template => template.Name.ToLowerInvariant().Contains(templateName.ToLowerInvariant() ?? string.Empty));
        if (searchNameResults.Count() == 1)
        {
            return searchNameResults;
        }
        
        // search for template via complete url
        IEnumerable<Template> searchUrlResults = templates.Where(template => template.Url.ToLowerInvariant() == templateName.ToLowerInvariant());
        if (searchUrlResults.Count() == 1)
        {
            return searchUrlResults;
        }

        // else return all templates
        return templates;
    }

    private async Task EnsureDefaultRepository(List<string> repositories)
    {
        if (!repositories.Contains(DEFAULT_TEMPLATE_REPOSITORY))
        {
            await this._configurationManager.AddAsync("template-repositories", DEFAULT_TEMPLATE_REPOSITORY);

            repositories.Add(DEFAULT_TEMPLATE_REPOSITORY);
        }
    }

    /// <inheritdoc />
    public async Task DownloadTemplateAsync(Template template)
    {
        string userCacheDirectoryPath = this._configuration.GetValue<string>("Core:UserCacheDirectoryPath");
        string templateCacheDirectory = $"{userCacheDirectoryPath}/templates";
        
        // create template directory in cache directory
        Directory.CreateDirectory(templateCacheDirectory);
        
        Console.WriteLine($"Download template '{Path.GetFileName(template.FileName)}'");
        await this._httpClientService.DownloadFileAsync(new Uri(template.Url), $"{templateCacheDirectory}/{Path.GetFileName(template.FileName)}");
        Console.WriteLine($"Download finished :)");
    }
}