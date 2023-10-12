using FluentAssertions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class CommentTagRendererTests
{
    private CommentTagRenderer _instance = null!;
    private IHtmlRenderer _htmlRenderer = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._htmlRenderer = Substitute.For<IHtmlRenderer>();
        
        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = @"C:\",
            SourceDirectory = @"C:\",
            OutputDirectory = @"C:\",
            CssOutputFilePath = @"C:\"
        };
        
        this._instance = new CommentTagRenderer(configuration,
            this._htmlRenderer);
    }

    [TestMethod]
    public async Task ReplaceCommentTags_BugWithHeaderInContent_Returns()
    {
        string sourceHtml = "<html>" + Environment.NewLine +
                            "<body>" + Environment.NewLine +
                            "@Comment=START body" + Environment.NewLine +
                            "<header>" + Environment.NewLine +
                            "<h1>hello world</h1>" + Environment.NewLine +
                            "</header>" + Environment.NewLine +
                            "@Comment=END body" + Environment.NewLine +
                            "</body>" + Environment.NewLine +
                            "</html>";
        string expectedHtml = "<html>" + Environment.NewLine +
                              "<body>" + Environment.NewLine +
                              "<!-- START body -->" + Environment.NewLine +
                              "<header>" + Environment.NewLine +
                              "<h1>hello world</h1>" + Environment.NewLine +
                              "</header>" + Environment.NewLine +
                              "<!-- END body -->" + Environment.NewLine +
                              "</body>" + Environment.NewLine +
                              "</html>";

        string html = await this._instance.RenderAsync(sourceHtml);

        html.Should().NotBeNullOrEmpty();
        html.Should().Be(expectedHtml);
    }
}