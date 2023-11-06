using FluentAssertions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class ImageStringRendererTests
{
    private ImageStringRenderer _instance = null!;
    private IHtmlRenderer _htmlRenderer = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._htmlRenderer = Substitute.For<IHtmlRenderer>();

        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = "/project/src",
            SourceDirectory = "/project/src",
            OutputDirectory = "/project/dist",
            CssOutputFilePath = ""
        };

        this._instance = new ImageStringRenderer(configuration,
            this._htmlRenderer);
    }

    [TestMethod]
    public async Task RenderAsync_WithSingleText()
    {
        // Arrange
        string content = "<p><img src=\"@ImageString(\"Dies ist ein test\")\" /></p>";

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        result.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task RenderAsync_WithTextAndColor()
    {
        // Arrange
        string content = "<p><img src=\"@ImageString(\"Dies ist ein test\", \"#ff0000\", \"#0000ff\")\" /></p>";

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        result.Should().NotBeEmpty();
    }
    
    [TestMethod]
    public async Task RenderAsync_WithTextAndColorAndFontSize()
    {
        // Arrange
        string content = "<p><img src=\"@ImageString(\"Dies ist ein test\", \"#ff0000\", \"#0000ff\", 40)\" /></p>";

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        result.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task RenderAsync_WithTextAndTransparentColor()
    {
        // Arrange
        string content = "<p><img src=\"@ImageString(\"Dies ist ein test\", \"#00ff0000\", \"#88000000\")\" /></p>";

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        result.Should().NotBeEmpty();
    }
}