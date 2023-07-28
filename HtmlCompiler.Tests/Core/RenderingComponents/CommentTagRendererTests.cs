using FluentAssertions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using Moq;

namespace HtmlCompiler.Tests.Core.RenderingComponents;

[TestClass]
public class CommentTagRendererTests
{
    private CommentTagRenderer _instance = null!;
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
        
        this._instance = new CommentTagRenderer(configuration,
            this._fileSystemService.Object,
            this._htmlRenderer.Object);
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