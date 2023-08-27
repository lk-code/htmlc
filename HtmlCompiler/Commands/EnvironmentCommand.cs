using Cocona;
using HtmlCompiler.Core.Exceptions;
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
        try
        {
            string output = await this._dependencyManager.CheckEnvironmentAsync();
            Console.WriteLine(output);
        }
        catch (Exception err)
        {
            Console.WriteLine("Error while dependency check:");
            Console.WriteLine(err.Message);
        }
    }

    [Command("setup")]
    public async Task Setup()
    {
        try
        {
            string output = await this._dependencyManager.SetupEnvironmentAsync();
            Console.WriteLine(output);
        }
        catch (Exception err)
        {
            Console.WriteLine("Error while dependency setup:");
            Console.WriteLine(err.Message);
        }
    }
}