using System.Text.RegularExpressions;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Dependencies;

public class SassDependency : IDependencyObject
{
    private readonly ICLIManager _cliManager;

    private const string SASS_VERSION_PATTERN = @"^\d+\.\d+\.\d+.*$";


    public string Name { get; } = "Sass Compiler";

    public List<IDependencyObject> Dependencies { get; } = new()
    {
        new NodeDependency(null!)
    };

    public SassDependency(ICLIManager cliManager)
    {
        _cliManager = cliManager;
    }

    public async Task<bool> CheckAsync()
    {
        await Task.CompletedTask;
        
        string result = string.Empty;

        try
        {
            result = _cliManager.ExecuteCommand("sass --version");
        }
        catch (ConsoleExecutionException err)
        {
            result = err.Message;
        }

        if (Regex.IsMatch(result, SASS_VERSION_PATTERN))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task SetupAsync()
    {
        await Task.CompletedTask;
        
        try
        {
            this._cliManager.ExecuteCommand("npm install -g sass");
        }
        catch (Exception err)
        {
            throw new DependencySetupFailedException("Setup Failure", err);
        }
    }
}