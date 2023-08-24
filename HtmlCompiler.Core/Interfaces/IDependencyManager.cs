namespace HtmlCompiler.Core.Interfaces;

public interface IDependencyManager
{
    Task<string> CheckEnvironmentAsync();
    Task<string> SetupEnvironmentAsync();
}