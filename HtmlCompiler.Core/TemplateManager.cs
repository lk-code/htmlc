using HtmlCompiler.Config;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Core;

public class TemplateManager : ITemplateManager
{
    private const string DEFAULT_TEMPLATE_REPOSITORY = "https://raw.githubusercontent.com/lk-code/htmlc-templates/main/";
    
    private readonly IConfiguration _configuration;
    private readonly IConfigurationManager _configurationManager;

    public TemplateManager(IConfiguration configuration,
        IConfigurationManager configurationManager)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
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

        throw new NotImplementedException();
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