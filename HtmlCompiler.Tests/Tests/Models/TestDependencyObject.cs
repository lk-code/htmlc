using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Tests.Tests.Models;

public class TestDependencyObject : IDependencyObject
{
    public string Name { get; }
    public List<IDependencyObject> Dependencies { get; }

    public TestDependencyObject(string name, List<IDependencyObject> dependencies)
    {
        this.Name = name;
        this.Dependencies = dependencies;
    }
    
    public Task<bool> CheckAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetupAsync()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return this.Name;
    }
}