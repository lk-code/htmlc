using System.Text;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using FluentDataBuilder.Microsoft.Extensions.Configuration;
using HtmlCompiler.Commands;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace HtmlCompiler.Tests.App.Commands;

[TestClass]
public class EnvironmentCommandTests
{
    private EnvironmentCommand _instance = null!;
    private IDependencyManager _dependencyManager = null!;

    [TestInitialize]
    public void SetUp()
    {
    }

    private void CreateTestInstance(IConfiguration configuration)
    {
        this._dependencyManager = Substitute.For<IDependencyManager>();
        
        this._instance = new EnvironmentCommand(configuration, this._dependencyManager);
    }

    [TestMethod]
    public async Task New_WithTemplateNameAndNoResult_DisplayError()
    {
        IDataBuilder dataBuilder = new DataBuilder();
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        this._instance.Check();
    }
}