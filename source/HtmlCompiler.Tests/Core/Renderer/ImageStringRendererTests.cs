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
    public async Task RenderAsync_ShouldReplaceFileTagWithRenderedContent()
    {
        // Arrange
        string content = "<p><img src=\"@ImageString(\"Dies ist ein test\")\" /></p>";

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        Assert.AreEqual("<p><img src=\"#\" /></p>", result);
    }
}