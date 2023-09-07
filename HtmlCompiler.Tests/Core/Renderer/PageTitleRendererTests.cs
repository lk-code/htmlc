using FluentAssertions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class PageTitleRendererTests
{
    private PageTitleRenderer _instance = null!;
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
        
        this._instance = new PageTitleRenderer(configuration,
            this._fileSystemService,
            this._htmlRenderer);
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