using FluentDataBuilder;
using FluentDataBuilder.Microsoft.Extensions.Configuration;
using HtmlCompiler.Commands;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HtmlCompiler.Tests.App.Commands;

[TestClass]
public class EnvironmentCommandTests
{
    private ILogger<EnvironmentCommands> _logger = null!;
    private EnvironmentCommands _instance = null!;
    private IDependencyManager _dependencyManager = null!;

    [TestInitialize]
    public void SetUp()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        ILoggerFactory factory = serviceProvider.GetService<ILoggerFactory>()!;

        this._logger = factory.CreateLogger<EnvironmentCommands>();
    }

    private void CreateTestInstance(IConfiguration configuration)
    {
        this._dependencyManager = Substitute.For<IDependencyManager>();
        
        this._instance = new EnvironmentCommands(this._logger, configuration, this._dependencyManager);
    }

    [TestMethod]
    public async Task New_WithTemplateNameAndNoResult_DisplayError()
    {
        IDataBuilder dataBuilder = new DataBuilder();
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        await this._instance.Check();
    }
}