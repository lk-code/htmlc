using FluentAssertions;
using HtmlAgilityPack;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using Moq;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class MetaTagRendererTests
{
    private IMetaTagRenderer _instance = null!;
    private Mock<IFileSystemService> _fileSystemService = null!;
    private Mock<IHtmlRenderer> _htmlRenderer = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = new Mock<IFileSystemService>();
        this._htmlRenderer = new Mock<IHtmlRenderer>();
        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = @"C:\",
            SourceDirectory = @"C:\",
            OutputDirectory = @"C:\",
            CssOutputFilePath = @"C:\"
        };
        
        this._instance = new MetaTagRenderer(configuration,
            this._fileSystemService.Object,
            this._htmlRenderer.Object);
    }
    [TestMethod]
    public void AddMetaTag_WithDefaultHtml_Returns()
    {
        string sourceHtml = "<html><head><title>hello world</title></head><body><h1>hello world</h1></body></html>";

        string html = this._instance.AddMetaTagToContent(sourceHtml, "generator", "htmlc test");

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").GetAttributeValue("content", "").Should().Be("htmlc test");
    }

    [TestMethod]
    public void AddMetaTag_WithoutHeadTag_Returns()
    {
        string sourceHtml = "<html><body><h1>hello world</h1></body></html>";

        string html = this._instance.AddMetaTagToContent(sourceHtml, "generator", "htmlc test");

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").GetAttributeValue("content", "").Should().Be("htmlc test");
    }

    [TestMethod]
    public void AddMetaTag_BugWithHeaderInContent_Returns()
    {
        string sourceHtml = "<html><head><title>hello world</title></head><body><header><h1>hello world</h1></header/><p>hello world</p></body></html>";

        string html = this._instance.AddMetaTagToContent(sourceHtml, "generator", "htmlc test");

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").GetAttributeValue("content", "").Should().Be("htmlc test");
    }
}