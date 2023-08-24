using Cocona;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Commands;

public class EnvironmentCommand
{
    private readonly IConfiguration _configuration;
    private readonly IDependencyManager _dependencyManager;

    public EnvironmentCommand(IConfiguration configuration,
        IDependencyManager dependencyManager)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _dependencyManager = dependencyManager ?? throw new ArgumentNullException(nameof(dependencyManager));
    }

    [Command("check")]
    public async Task Check()
    {
        await this._dependencyManager.CheckEnvironmentAsync();
    }

    [Command("setup")]
    public async Task Setup()
    {
        
    }
}