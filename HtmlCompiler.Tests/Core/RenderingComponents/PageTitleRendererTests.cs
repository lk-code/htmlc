using FluentAssertions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using Moq;

namespace HtmlCompiler.Tests.Core.RenderingComponents;

[TestClass]
public class PageTitleRendererTests
{
    private PageTitleRenderer _instance = null!;
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
        
        this._instance = new PageTitleRenderer(configuration,
            this._fileSystemService.Object,
            this._htmlRenderer.Object);
    }

    [TestMethod]
    public async Task RenderPageTitle_WithDefault_Returns()
    {
        string html = "@PageTitle=Test Page :D" + Environment.NewLine +
                      "<section>" + Environment.NewLine +
                      "<h1>@PageTitle</h1>" + Environment.NewLine +
                      "</section>";
        string expectedHtml = "<section>" + Environment.NewLine +
                              "<h1>Test Page :D</h1>" + Environment.NewLine +
                              "</section>";

        string result = await this._instance.RenderAsync(html);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }
}