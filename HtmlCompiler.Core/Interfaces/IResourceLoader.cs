namespace HtmlCompiler.Core.Interfaces;

public interface IResourceLoader
{
    Task<string?> GetResourceContentAsync(string resourceName);
}