using System.Text.RegularExpressions;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Dependencies;

public class LessDependency : IDependencyObject
{
    private readonly ICLIManager _cliManager;
    
    private const string LESS_VERSION_PATTERN = @"^less \d+\.\d+.*$";

    public string Name { get; } = "Less Compiler";

    public List<IDependencyObject> Dependencies { get; } = new()
    {
        new NodeDependency(null!)
    };

    public LessDependency(ICLIManager cliManager)
    {
        _cliManager = cliManager;
    }

    public async Task<bool> CheckAsync()
    {
        await Task.CompletedTask;

        string result = string.Empty;

        try
        {
            result = _cliManager.ExecuteCommand("less --version");
            string[] resultLines = result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            result = resultLines[0];
        }
        catch (ConsoleExecutionException err)
        {
            result = err.Message;
        }

        if (Regex.IsMatch(result, LESS_VERSION_PATTERN, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
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
            this._cliManager.ExecuteCommand("npm install -g less");
        }
        catch (Exception err)
        {
            throw new DependencySetupFailedException("Setup Failure", err);
        }
    }
}