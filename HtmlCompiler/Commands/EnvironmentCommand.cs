using Cocona;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Commands;

public class EnvironmentCommand
{
    private readonly ILogger<EnvironmentCommand> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDependencyManager _dependencyManager;

    public EnvironmentCommand(ILogger<EnvironmentCommand> logger,
        IConfiguration configuration,
        IDependencyManager dependencyManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _dependencyManager = dependencyManager ?? throw new ArgumentNullException(nameof(dependencyManager));
    }

    [Command("check")]
    public async Task Check()
    {
        try
        {
            string output = await this._dependencyManager.CheckEnvironmentAsync();
            this._logger.LogInformation(output);
        }
        catch (Exception err)
        {
            this._logger.LogError(err, "Error while dependency check");
        }
    }

    [Command("setup")]
    public async Task Setup()
    {
        try
        {
            string output = await this._dependencyManager.SetupEnvironmentAsync();
            this._logger.LogInformation(output);
        }
        catch (Exception err)
        {
            this._logger.LogError(err, "Error while dependency setup");
        }
    }
}