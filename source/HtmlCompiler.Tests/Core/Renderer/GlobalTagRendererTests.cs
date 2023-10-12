using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class GlobalTagRendererTests
{
    private GlobalTagRenderer _instance = null!;
    private IHtmlRenderer _htmlRenderer = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._htmlRenderer = Substitute.For<IHtmlRenderer>();
    }

    private void CreateTestInstance(RenderingConfiguration configuration)
    {
        this._instance = new GlobalTagRenderer(configuration,
            this._htmlRenderer);
    }

    [TestMethod]
    public async Task RenderAsync_WithSimpleGlobalVariable_Returns()
    {
        // Arrange
        string content = "Some content @Global:ApplicationName here.";

        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = "/project/src",
            SourceDirectory = "/project/src",
            OutputDirectory = "/project/dist",
            CssOutputFilePath = "",
            GlobalVariables = new DataBuilder()
                .Add("ApplicationName", "test-app").Build().RootElement
        };

        this.CreateTestInstance(configuration);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        Assert.AreEqual("Some content test-app here.", result);
    }

    [TestMethod]
    public async Task RenderAsync_WithMultipleSimpleGlobalVariable_Returns()
    {
        // Arrange
        string content = "Some content @Global:ApplicationName and @Global:TestValue here.";

        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = "/project/src",
            SourceDirectory = "/project/src",
            OutputDirectory = "/project/dist",
            CssOutputFilePath = "",
            GlobalVariables = new DataBuilder()
                .Add("ApplicationName", "test-app")
                .Add("TestValue", 4488).Build().RootElement
        };

        this.CreateTestInstance(configuration);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        Assert.AreEqual("Some content test-app and 4488 here.", result);
    }

    [TestMethod]
    public async Task RenderAsync_WithMultiLevelGlobalVariable_Returns()
    {
        // Arrange
        string content = "Some content @Global:Application:Name here.";

        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = "/project/src",
            SourceDirectory = "/project/src",
            OutputDirectory = "/project/dist",
            CssOutputFilePath = "",
            GlobalVariables = new DataBuilder()
                .Add("Application", new DataBuilder()
                    .Add("Name", "test-app")
                ).Build().RootElement
        };

        this.CreateTestInstance(configuration);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        Assert.AreEqual("Some content test-app here.", result);
    }

    [TestMethod]
    public async Task RenderAsync_WithNotExistingKey_Returns()
    {
        // Arrange
        string content = "Some content @Global:Application:Name here.";

        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = "/project/src",
            SourceDirectory = "/project/src",
            OutputDirectory = "/project/dist",
            CssOutputFilePath = "",
            GlobalVariables = new DataBuilder().Build().RootElement
        };

        this.CreateTestInstance(configuration);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        Assert.AreEqual("Some content @Global:Application:Name here.", result);
    }

    [TestMethod]
    public async Task RenderAsync_WithMultipleAndMultiLevelGlobalVariable_Returns()
    {
        // Arrange
        string content = "Some content @Global:Application:Name and @Global:TestValue here.";

        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = "/project/src",
            SourceDirectory = "/project/src",
            OutputDirectory = "/project/dist",
            CssOutputFilePath = "",
            GlobalVariables = new DataBuilder()
                .Add("Application", new DataBuilder()
                    .Add("Name", "test-app"))
                .Add("TestValue", 4488).Build().RootElement
        };

        this.CreateTestInstance(configuration);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        Assert.AreEqual("Some content test-app and 4488 here.", result);
    }

    [TestMethod]
    public async Task RenderAsync_WithoutSpaceAfterKeyName_Returns()
    {
        // Arrange
        string content = "Some content <h1>@Global:Application:Name</h1> (v@Global:Application:Version) here.";

        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = "/project/src",
            SourceDirectory = "/project/src",
            OutputDirectory = "/project/dist",
            CssOutputFilePath = "",
            GlobalVariables = new DataBuilder()
                .Add("Application", new DataBuilder()
                    .Add("Name", "test-app")
                    .Add("Version", "1.0")).Build().RootElement
        };

        this.CreateTestInstance(configuration);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        Assert.AreEqual("Some content <h1>test-app</h1> (v1.0) here.", result);
    }
}