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
            ))
            .Where(template => template.Name.ToLowerInvariant().Contains(templateName.ToLowerInvariant() ?? string.Empty));

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
    public Task DownloadTemplateAsync(Template template)
    {
        throw new NotImplementedException();
    }
}