using Cocona;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Commands;

[HasSubCommands(typeof(TemplateCommands), "template", Description = "template sub-commands")]
public class TemplateRootCommand
{
}

public class TemplateCommands
{
    private readonly ILogger<TemplateCommands> _logger;
    private readonly IConfiguration _configuration;
    private readonly ITemplatePackagingService _templatePackagingService;

    public TemplateCommands(ILogger<TemplateCommands> logger,
        IConfiguration configuration,
        ITemplatePackagingService templatePackagingService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _templatePackagingService = templatePackagingService ?? throw new ArgumentNullException(nameof(templatePackagingService));
    }
    
    [Command("create")]
    public async Task Create(
        [Argument(Description = "path to the project directory.")]
        string sourcePath,
        [Argument(Description = "path to the output directory where the ZIP file should be saved. if empty, the ZIP file is saved directly in the source directory.")]
        string? outputPath = null)
    {
        try
        {
            await _templatePackagingService.CreateAsync(sourcePath, outputPath);
        }
        catch (TemplateFileException err)
        {
            _logger.LogError(err, "An error occurred while add files to the template:");
            foreach (string error in err.Errors)
            {
                _logger.LogError(error);
            }
        }
        catch (Exception err)
        {
            _logger.LogError(err, "An error occurred while creating the template.");
        }
    }
    
    // public async Task Upload()
    // {
    //     
    // }
}