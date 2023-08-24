using System.Text.RegularExpressions;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Dependencies;

public class NodeDependency : IDependencyObject
{
    private readonly ICLIManager _cliManager;
    
    private const string NODE_VERSION_PATTERN = @"^v\d{1,3}\.\d{1,3}\.\d{1,3}$";

    public string Name { get; } = "NodeJS";
    public List<IDependencyObject> Dependencies { get; } = new();

    public NodeDependency(ICLIManager cliManager)
    {
        _cliManager = cliManager;
    }

    public async Task<bool> CheckAsync()
    {
        string result = string.Empty;

        try
        {
            result = _cliManager.ExecuteCommand("nope --version");
        }
        catch (ConsoleExecutionException err)
        {
            result = err.Message;
        }

        if (Regex.IsMatch(result, NODE_VERSION_PATTERN))
        {
            return true;
        }
        else
        {
            // throw exception to abort the following checks
            throw new DependencyCheckFailedException("Please install NodeJS from the official website and try again :)");
        }
    }

    public async Task<bool> SetupAsync()
    {
        throw new DependencySetupFailedException("Please install NodeJS from the official website and try again :)");
    }
}