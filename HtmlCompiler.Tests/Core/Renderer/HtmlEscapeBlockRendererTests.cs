using FluentAssertions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class HtmlEscapeBlockRendererTests
{
    private HtmlEscapeBlockRenderer _instance = null!;
    private IFileSystemService _fileSystemService = null!;
    private IHtmlRenderer _htmlRenderer = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = Substitute.For<IFileSystemService>();
        this._htmlRenderer = Substitute.For<IHtmlRenderer>();
        
        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = @"C:\",
            SourceDirectory = @"C:\",
            OutputDirectory = @"C:\",
            CssOutputFilePath = @"C:\"
        };
        
        this._instance = new HtmlEscapeBlockRenderer(configuration,
            this._fileSystemService,
            this._htmlRenderer);
    }

    [TestMethod]
    public async Task RenderHtmlEscapeBlocks_WithSimpleHtml_Returns()
    {
        string html = "<section>" + Environment.NewLine +
                      "@StartHtmlSpecialChars" + Environment.NewLine +
                      "<h1 class=\"title\">" + Environment.NewLine +
                      "<a href=\"https://a.link.testcase\">a link</a>" + Environment.NewLine +
                      "</h1>" + Environment.NewLine +
                      "@EndHtmlSpecialChars" + Environment.NewLine +
                      "</section>";
        string expectedHtml = "<section>" + Environment.NewLine +
                              "<br>" + Environment.NewLine +
                              "&#60;h1 class=&#34;title&#34;&#62;<br>" + Environment.NewLine +
                              "&#60;a href=&#34;https://a.link.testcase&#34;&#62;a link&#60;/a&#62;<br>" + Environment.NewLine +
                              "&#60;/h1&#62;<br>" + Environment.NewLine +
                              "" + Environment.NewLine +
                              "</section>";

        string result = await this._instance.RenderAsync(html);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public async Task RenderHtmlEscapeBlocks_WithMultipleTags_Returns()
    {
        string html = "<section>" + Environment.NewLine +
                      "@StartHtmlSpecialChars" + Environment.NewLine +
                      "<h1 class=\"title\">" + Environment.NewLine +
                      "<a href=\"https://a.link.testcase\">a link</a>" + Environment.NewLine +
                      "</h1>" + Environment.NewLine +
                      "@EndHtmlSpecialChars" + Environment.NewLine +
                      "</section>" + Environment.NewLine +
                      "<section>" + Environment.NewLine +
                      "@StartHtmlSpecialChars" + Environment.NewLine +
                      "<h1 class=\"title\">" + Environment.NewLine +
                      "<a href=\"https://a.link.testcase\">a link</a>" + Environment.NewLine +
                      "</h1>" + Environment.NewLine +
                      "@EndHtmlSpecialChars" + Environment.NewLine +
                      "</section>";
        string expectedHtml = "<section>" + Environment.NewLine +
                              "<br>" + Environment.NewLine +
                              "&#60;h1 class=&#34;title&#34;&#62;<br>" + Environment.NewLine +
                              "&#60;a href=&#34;https://a.link.testcase&#34;&#62;a link&#60;/a&#62;<br>" + Environment.NewLine +
                              "&#60;/h1&#62;<br>" + Environment.NewLine +
                              "" + Environment.NewLine +
                              "</section>" + Environment.NewLine +
                              "<section>" + Environment.NewLine +
                              "<br>" + Environment.NewLine +
                              "&#60;h1 class=&#34;title&#34;&#62;<br>" + Environment.NewLine +
                              "&#60;a href=&#34;https://a.link.testcase&#34;&#62;a link&#60;/a&#62;<br>" + Environment.NewLine +
                              "&#60;/h1&#62;<br>" + Environment.NewLine +
                              "" + Environment.NewLine +
                              "</section>";

        string result =await this._instance.RenderAsync(html);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }
}