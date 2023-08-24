using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Dependencies;

public class NodeDependency : IDependencyObject
{
    private readonly ICLIManager _cliManager;

    public string Name { get; } = "NodeJS";
    public List<IDependencyObject> Dependencies { get; } = new();

    public NodeDependency(ICLIManager cliManager)
    {
        _cliManager = cliManager;
    }

    public async Task<bool> CheckAsync()
    {
        string result = _cliManager.ExecuteCommand("node --version");

        string pattern = @"^v\d{1,3}\.\d{1,3}\.\d{1,3}$";
        if (Regex.IsMatch(result, pattern))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> SetupAsync()
    {
        throw new NotImplementedException();
    }
}