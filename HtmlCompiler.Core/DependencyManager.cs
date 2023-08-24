using System.Text;
using HtmlCompiler.Core.Exceptions;
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

    public async Task<string> CheckEnvironmentAsync()
    {
        return await this.CheckEnvironmentAndSetupAsync(false);
    }

    public async Task<string> SetupEnvironmentAsync()
    {
        return await this.CheckEnvironmentAndSetupAsync();
    }

    private async Task<string> CheckEnvironmentAndSetupAsync(bool setup = true)
    {
        StringBuilder checkConsoleOutput = new StringBuilder();
        
        List<IDependencyObject> dependencies = ResolveDependencies(this._dependencies)
            .ToList();

        foreach (IDependencyObject dependency in dependencies)
        {
            try
            {
                bool isInstalled = await dependency.CheckAsync();
                if (isInstalled)
                {
                    checkConsoleOutput.AppendLine($"{dependency.Name} is installed");
                }
                else
                {
                    checkConsoleOutput.AppendLine($"{dependency.Name} is NOT installed");
                }

                if (setup)
                {
                    checkConsoleOutput.AppendLine($"Try to install {dependency.Name}");
                    
                    bool setupSuccessful = await dependency.SetupAsync();
                    if (!setupSuccessful)
                    {
                        checkConsoleOutput.AppendLine($"Setup for {dependency.Name} failed");
                    }
                }

            }
            catch (DependencyCheckFailedException err)
            {
                checkConsoleOutput.AppendLine($"Dependency Check Failure for {dependency.Name}:");
                checkConsoleOutput.AppendLine(err.Message);

                break;
            }
            catch (DependencySetupFailedException err)
            {
                checkConsoleOutput.AppendLine($"Dependency Setup Failure for {dependency.Name}:");
                checkConsoleOutput.AppendLine(err.Message);

                break;
            }
        }

        return checkConsoleOutput.ToString();
    }

    public static IEnumerable<IDependencyObject> ResolveDependencies(IEnumerable<IDependencyObject> dependencies)
    {
        Dictionary<Type, IDependencyObject> uniqueDependencies = new Dictionary<Type, IDependencyObject>();
        HashSet<IDependencyObject> visited = new HashSet<IDependencyObject>();

        foreach (var dependency in dependencies)
        {
            VisitDependency(dependency, uniqueDependencies, visited);
        }

        List<IDependencyObject> resolvedDependencies = uniqueDependencies.Values.ToList();
        List<IDependencyObject> result = new List<IDependencyObject>();
        foreach (var uniqueDependency in resolvedDependencies)
        {
            result.Add(dependencies.First(x => x.GetType() == uniqueDependency.GetType()));
        }

        return result;
    }

    private static void VisitDependency(IDependencyObject dependency,
        Dictionary<Type, IDependencyObject> uniqueDependencies, HashSet<IDependencyObject> visited)
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