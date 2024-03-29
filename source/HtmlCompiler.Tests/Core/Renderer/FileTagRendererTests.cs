using System.Text.Json;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class FileTagRendererTests
{
    private FileTagRenderer _instance = null!;
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

        this._instance = new FileTagRenderer(configuration,
            this._htmlRenderer);
    }

    [TestMethod]
    public async Task RenderAsync_ShouldReplaceFileTagWithRenderedContent()
    {
        // Arrange
        string content = "Some content @File=example.txt here.";
        string fileContent = "Rendered file content";

        // Mock the FileExists method to return true for this test
        // this._htmlRenderer.FileSystemService.FileExists(Arg.Any<string>()).Returns(true);

        this._htmlRenderer.RenderHtmlFromFileAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<JsonElement?>(),
                Arg.Any<long>())
            .Returns(fileContent);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        Assert.AreEqual("Some content Rendered file content here.", result);
    }
}