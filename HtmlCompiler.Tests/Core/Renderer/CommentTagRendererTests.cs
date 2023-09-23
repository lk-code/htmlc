using FluentAssertions;
using HtmlCompiler.Commands;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class CommentTagRendererTests
{
    private ILogger<CommentTagRenderer> _logger = null!;
    private CommentTagRenderer _instance = null!;
    private IFileSystemService _fileSystemService = null!;
    private IHtmlRenderer _htmlRenderer = null!;

    [TestInitialize]
    public void SetUp()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        ILoggerFactory? factory = serviceProvider.GetService<ILoggerFactory>();

        this._logger = factory.CreateLogger<CommentTagRenderer>();
        
        this._fileSystemService = Substitute.For<IFileSystemService>();
        this._htmlRenderer = Substitute.For<IHtmlRenderer>();
        
        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = @"C:\",
            SourceDirectory = @"C:\",
            OutputDirectory = @"C:\",
            CssOutputFilePath = @"C:\"
        };
        
        this._instance = new CommentTagRenderer(this._logger,
            configuration,
            this._fileSystemService,
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