namespace HtmlCompiler.Core.Interfaces;

public interface IDependencyObject
{
    string Name { get; }
    List<IDependencyObject> Dependencies { get; }
    Task<bool> CheckAsync();
    Task SetupAsync();
}