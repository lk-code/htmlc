using System.Text;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using Moq;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class LayoutRendererTests
{
    private LayoutRenderer _instance = null!;
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

        this._instance = new LayoutRenderer(configuration,
            this._fileSystemService.Object,
            this._htmlRenderer.Object);
    }

    [TestMethod]
    public async Task RenderAsync_WithNonExistingLayout_Throws()
    {
        var sourceHtml = new StringBuilder()
            .AppendLine("@Layout=_unknownlayout.html")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();

        this._fileSystemService.Setup(x => x.FileReadAllTextAsync("/project/src/_unknownlayout.html"))
            .Throws(new FileNotFoundException());

        Assert.ThrowsExceptionAsync<FileNotFoundException>(() => this._instance.RenderAsync(sourceHtml));
    }

    [TestMethod]
    public async Task RenderAsync_WithoutLayoutTag_Returns()
    {
        var sourceHtml = new StringBuilder()
            .AppendLine("@Layout=_layout.html")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();

        this._fileSystemService.Setup(x => x.FileReadAllTextAsync("/project/src/_layout.html"))
            .ReturnsAsync("<section>Hello World!</section>");
        
        string result = await this._instance.RenderAsync(sourceHtml);
        
        Assert.AreEqual(result, "<h1>Hello World!</h1>");
    }
}