using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Core;

public class TemplateManager : ITemplateManager
{
    private readonly IConfiguration _configuration;

    public TemplateManager(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Template>> SearchTemplatesAsync(string templateName)
    {
        string[] repositories = this._configuration.GetSection("TemplateRepositories").Get<string[]>();

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task DownloadTemplateAsync(Template template)
    {
        throw new NotImplementedException();
    }
}