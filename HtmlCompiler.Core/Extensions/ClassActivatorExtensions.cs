using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core.Extensions;

public static class ClassActivatorExtensions
{
    /// <summary>
    /// Builds a list of instances of type T for the given rendering components.
    /// </summary>
    /// <typeparam name="T">The type of instances to create, must implement the IRenderingComponent interface.</typeparam>
    /// <param name="renderingComponents">A list of types from which the instances should be created.</param>
    /// <param name="configuration">The rendering configuration to be passed to the constructor of the instances.</param>
    /// <returns>A list of instances of type T that implement the IRenderingComponent interface and are configured accordingly.</returns>
    public static IEnumerable<IRenderingComponent> BuildRenderingComponents(this IEnumerable<Type> renderingComponents,
        ILogger<IRenderingComponent> logger,
        RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
    {
        List<IRenderingComponent> instances = new List<IRenderingComponent>();

        foreach (Type type in renderingComponents)
        {
            if (typeof(IRenderingComponent).IsAssignableFrom(type))
            {
                IRenderingComponent instance = (IRenderingComponent)Activator.CreateInstance(type,
                    logger,
                    configuration,
                    fileSystemService,
                    htmlRenderer)!;
                instances.Add(instance);
            }
        }

        return instances;
    }
}