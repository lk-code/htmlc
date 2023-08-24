using FluentAssertions;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Dependencies;
using HtmlCompiler.Core.Interfaces;
using NSubstitute;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class DependencyManagerTests
{
    private DependencyManager _instance = null!;
    private ICLIManager _cliManager = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._cliManager = Substitute.For<ICLIManager>();
    }

    private void CreateTestInstance(IEnumerable<IDependencyObject> dependencies)
    {
        this._instance = new DependencyManager(this._cliManager, dependencies);
    }

    [TestMethod]
    public void ResolveDependencies_WithRealList_Return()
    {
        var dependencies = new List<IDependencyObject>
        {
            new SassDependency(null),
            new LessDependency(null),
            new NodeDependency(null)
        };

        var resolveDependencies = DependencyManager.ResolveDependencies(dependencies).ToList();

        resolveDependencies.Should().NotBeNull();
        resolveDependencies.Should().HaveCount(3);
        resolveDependencies[0].GetType().Should().Be<NodeDependency>();
        resolveDependencies[1].GetType().Should().Be<SassDependency>();
        resolveDependencies[2].GetType().Should().Be<LessDependency>();
    }

    [TestMethod]
    public async Task CheckEnvironmentAsync_WithRealList_Return()
    {
        var dependencies = new List<IDependencyObject>
        {
            new NodeDependency(this._cliManager),
            new SassDependency(this._cliManager),
            new LessDependency(this._cliManager),
        };
        this.CreateTestInstance(dependencies);

        await this._instance.CheckEnvironmentAsync();
    }
}