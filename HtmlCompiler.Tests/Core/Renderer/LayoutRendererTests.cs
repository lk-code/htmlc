using System.Text;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class LayoutRendererTests
{
    private LayoutRenderer _instance = null!;
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

        this._instance = new LayoutRenderer(configuration,
            this._htmlRenderer);
    }

    [TestMethod]
    public void RenderAsync_WithNonExistingLayout_Throws()
    {
        var sourceHtml = new StringBuilder()
            .AppendLine("@Layout=_unknownlayout.html")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();

        this._htmlRenderer.FileSystemService.FileReadAllTextAsync("/project/src/_unknownlayout.html")
            .ThrowsAsync(new FileNotFoundException());

        Assert.ThrowsExceptionAsync<FileNotFoundException>(() => this._instance.RenderAsync(sourceHtml));
    }

    [TestMethod]
    public async Task RenderAsync_WithoutLayoutTag_Returns()
    {
        var sourceHtml = new StringBuilder()
            .AppendLine("@Layout=_layout.html")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();

        this._htmlRenderer.FileSystemService.FileReadAllTextAsync("/project/src/_layout.html")
            .Returns("<section>Hello World!</section>");
        
        string result = await this._instance.RenderAsync(sourceHtml);
        
        Assert.AreEqual(result, "<h1>Hello World!</h1>");
    }
}