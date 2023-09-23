using System.Text;
using FluentAssertions;
using HtmlAgilityPack;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class MarkdownFileTagRendererTests
{
    private MarkdownFileTagRenderer _instance = null!;
    private IFileSystemService _fileSystemService = null!;
    private IHtmlRenderer _htmlRenderer = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = Substitute.For<IFileSystemService>();
        this._htmlRenderer = Substitute.For<IHtmlRenderer>();
        
        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = "/project/src",
            SourceDirectory = "/project/src",
            OutputDirectory = "/project/dist",
            CssOutputFilePath = ""
        };

        this._instance = new MarkdownFileTagRenderer(configuration,
            this._fileSystemService,
            this._htmlRenderer);
    }

    [TestMethod]
    public async Task RenderAsync_WithMarkdownContent_Returns()
    {
        // Arrange
        var content = new StringBuilder()
            .AppendLine("<section><p>this is markdown</p> @MarkdownFile=mark-down.md <p>here.</p></section>")
            .AppendLine("<div class=\"caption\">with content</div>")
            .ToString().Trim();
        var markdownContent = new StringBuilder()
            .AppendLine("# Hello World")
            .AppendLine("## from htmlc")
            .AppendLine("")
            .AppendLine("this *is* a **test**")
            .ToString().Trim();

        // Mock the FileExists method to return true for this test
        this._fileSystemService.FileExists(Arg.Any<string>())
            .Returns(true);
        
        this._fileSystemService.FileReadAllTextAsync("/project/src/mark-down.md")
            .Returns(markdownContent);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(result);
        
        htmlDoc.DocumentNode.SelectSingleNode("section").Should().NotBeNull();
        
        htmlDoc.DocumentNode.SelectSingleNode("//section/p[1]").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//section/p[1]").GetDirectInnerText().Should().Be("this is markdown");
        
        htmlDoc.DocumentNode.SelectSingleNode("//section/p[3]").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//section/p[3]").GetDirectInnerText().Should().Be("here.");
        
        htmlDoc.DocumentNode.SelectSingleNode("//section/h1").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//section/h1").GetDirectInnerText().Should().Be("Hello World");
        
        htmlDoc.DocumentNode.SelectSingleNode("//section/h2").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//section/h2").GetDirectInnerText().Should().Be("from htmlc");
        
        htmlDoc.DocumentNode.SelectSingleNode("//section/p[2]").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//section/p[2]").InnerText.Should().Be("this is a test");
        
        htmlDoc.DocumentNode.SelectSingleNode("//section/p/em").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//section/p/em").GetDirectInnerText().Should().Be("is");
        
        htmlDoc.DocumentNode.SelectSingleNode("//section/p/strong").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//section/p/strong").GetDirectInnerText().Should().Be("test");
        
        htmlDoc.DocumentNode.SelectSingleNode("//div[@class=\"caption\"]").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//div[@class=\"caption\"]").GetDirectInnerText().Should().Be("with content");
    }

    [TestMethod]
    public async Task RenderAsync_WithNotExistingMarkdownFile_Returns()
    {
        // Arrange
        var content = new StringBuilder()
            .AppendLine("<section><p>this is markdown</p> @MarkdownFile=mark-down.md <p>here.</p></section>")
            .ToString().Trim();

        // Mock the FileExists method to return true for this test
        this._fileSystemService.FileExists(Arg.Any<string>())
            .Returns(false);

        // Act
        string result = await this._instance.RenderAsync(content);

        // Assert
        result.Should().Be(content);
    }
}