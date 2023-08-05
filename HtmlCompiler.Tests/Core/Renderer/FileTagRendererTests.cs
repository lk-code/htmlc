using System.Text;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using Moq;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class FileTagRendererTests
{
    private FileTagRenderer _instance = null!;
    private Mock<IFileSystemService> _fileSystemService = null!;
    private Mock<IHtmlRenderer> _htmlRenderer = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = new Mock<IFileSystemService>();
        this._htmlRenderer = new Mock<IHtmlRenderer>();
        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = "/project/src",
            SourceDirectory = "/project/src",
            OutputDirectory = "/project/dist",
            CssOutputFilePath = ""
        };

        this._instance = new FileTagRenderer(configuration,
            this._fileSystemService.Object,
            this._htmlRenderer.Object);
    }

    [TestMethod]
    public async Task RenderAsync_ShouldReplaceFileTagWithRenderedContent()
    {
        // Arrange
        string content = "Some content @File=example.txt here.";
        string fileContent = "Rendered file content";

        // Mock the FileExists method to return true for this test
        this._fileSystemService.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);

        this._htmlRenderer.Setup(r => r.RenderHtmlAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .ReturnsAsync(fileContent);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        Assert.AreEqual("Some content Rendered file content here.", result);
    }
}