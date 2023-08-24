using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Dependencies;

public class SassDependency : IDependencyObject
{
    private readonly ICLIManager _cliManager;

    public string Name { get; } = "Sass Compiler";

    public List<IDependencyObject> Dependencies { get; } = new()
    {
        new NodeDependency(null)
    };

    public SassDependency(ICLIManager cliManager)
    {
        _cliManager = cliManager;
    }

    public async Task<bool> CheckAsync()
    {
        Console.WriteLine("Checking Sass Compiler");

        return true;
    }

    public async Task<bool> SetupAsync()
    {
        throw new NotImplementedException();
    }
}