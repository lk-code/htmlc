using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class DependencyManager : IDependencyManager
{
    private readonly ICLIManager _cliManager;
    private readonly IEnumerable<IDependencyObject> _dependencies;

    public DependencyManager(ICLIManager cliManager,
        IEnumerable<IDependencyObject> dependencies)
    {
        _cliManager = cliManager ?? throw new ArgumentNullException(nameof(cliManager));
        _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
    }

    public async Task CheckEnvironmentAsync()
    {
        List<IDependencyObject> dependencies = ResolveDependencies(this._dependencies)
            .ToList();

        foreach (IDependencyObject dependency in dependencies)
        {
            await dependency.CheckAsync();
        }

        string result = this._cliManager.ExecuteCommand("dotnet --info");
        int i = 0;
    }

    public static IEnumerable<IDependencyObject> ResolveDependencies(IEnumerable<IDependencyObject> dependencies)
    {
        var uniqueDependencies = new Dictionary<Type, IDependencyObject>();
        var visited = new HashSet<IDependencyObject>();

        foreach (var dependency in dependencies)
        {
            VisitDependency(dependency, uniqueDependencies, visited);
        }

        return uniqueDependencies.Values.ToList();
    }

    private static void VisitDependency(IDependencyObject dependency, Dictionary<Type, IDependencyObject> uniqueDependencies, HashSet<IDependencyObject> visited)
    {
        if (!visited.Contains(dependency))
        {
            visited.Add(dependency);

            foreach (var childDependency in dependency.Dependencies)
            {
                VisitDependency(childDependency, uniqueDependencies, visited);
            }

            if (!uniqueDependencies.ContainsKey(dependency.GetType()))
            {
                uniqueDependencies.Add(dependency.GetType(), dependency);
            }
        }
    }

}