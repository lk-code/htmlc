using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Dependencies;

public class LessDependency : IDependencyObject
{
    private readonly ICLIManager _cliManager;

    public string Name { get; } = "Less Compiler";

    public List<IDependencyObject> Dependencies { get; } = new()
    {
        new NodeDependency(null)
    };

    public LessDependency(ICLIManager cliManager)
    {
        _cliManager = cliManager;
    }

    public async Task<bool> CheckAsync()
    {
        Console.WriteLine("Checking Less Compiler");

        return true;
    }

    public async Task<bool> SetupAsync()
    {
        throw new NotImplementedException();
    }
}