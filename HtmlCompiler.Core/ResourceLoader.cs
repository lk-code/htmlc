using System.Reflection;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class ResourceLoader : IResourceLoader
{
    public async Task<string?> GetResourceContentAsync(string resourceName)
    {
        string? content = null;
        
        Assembly assembly = Assembly.GetExecutingAssembly();

        using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
        using (StreamReader reader = new StreamReader(stream))
        {
            content = await reader.ReadToEndAsync();
        }

        return content;
    }
}