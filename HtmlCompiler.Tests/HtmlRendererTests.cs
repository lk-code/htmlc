using System.Text;
using AdvancedStringBuilder;
using FluentAssertions;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Moq;

namespace HtmlCompiler.Tests;

[TestClass]
public class HtmlRendererTests
{
    private HtmlRenderer _instance = null!;
    private Mock<IFileSystemService> _fileSystemService = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = new Mock<IFileSystemService>();
        
        this._instance = new HtmlRenderer(this._fileSystemService.Object);
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

        string result = await this._instance.ReplacePageTitlePlaceholderAsync(html);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public async Task RenderHtmlAsync_WithSimpleFileWithoutTags_Return()
    {
        string sourceFullFilePath = "/project/src/index.html";
        string sourceDirectory = "/project/src";
        string outputDirectory = "/project/dist";
        string? cssOutputFilePath = null;
        
        string expectedHtml = "Hello World!";

        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/index.html"))
            .ReturnsAsync("Hello World!");
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public async Task RenderHtmlAsync_WithSimpleLayout_Return()
    {
        string sourceFullFilePath = "/project/src/index.html";
        string sourceDirectory = "/project/src";
        string outputDirectory = "/project/dist";
        string? cssOutputFilePath = null;
        
        string expectedHtml = "<html><body><h1>Hello World!</h1></body><head><meta name=\"generator\" content=\"htmlc\"></head></html>";

        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/index.html"))
            .ReturnsAsync("@Layout=_layoutbase.html" + Environment.NewLine + "<h1>Hello World!</h1>");
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html"))
            .ReturnsAsync("<html><body>@Body</body></html>");
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public async Task RenderHtmlAsync_WithPageTitle_Return()
    {
        string sourceFullFilePath = "/project/src/index.html";
        string sourceDirectory = "/project/src";
        string outputDirectory = "/project/dist";
        string? cssOutputFilePath = null;
        
        var expectedHtml = new StringBuilder()
            .Append("<html>")
            .Append("<head>")
            .Append("<title>Demo</title>")
            .Append("<meta name=\"generator\" content=\"htmlc\">")
            .Append("</head>")
            .Append("<body>")
            .Append("<h1>Hello World!</h1>")
            .Append("</body>")
            .Append("</html>")
            .ToString().Trim();

        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=Demo")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/index.html"))
            .ReturnsAsync(indexContent);
        
        var layoutContent = new StringBuilder()
            .Append("<html>")
            .Append("<head>")
            .Append("<title>@PageTitle</title>")
            .Append("</head>")
            .Append("<body>")
            .Append("@Body")
            .Append("</body>")
            .Append("</html>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html"))
            .ReturnsAsync(layoutContent);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public async Task RenderHtmlAsync_WithStylesheet_Return()
    {
        string sourceFullFilePath = "/project/src/index.html";
        string sourceDirectory = "/project/src";
        string outputDirectory = "/project/dist";
        string? cssOutputFilePath = "/css/site.scss";
        
        var expectedHtml = new StringBuilder()
            .Append("<html>")
            .Append("<head>")
            .Append("<title>Demo</title>")
            .Append("<link rel=\"stylesheet\" href=\"../../css/site.scss\">")
            .Append("<meta name=\"generator\" content=\"htmlc\">")
            .Append("</head>")
            .Append("<body>")
            .Append("<h1>Hello World!</h1>")
            .Append("</body>")
            .Append("</html>")
            .ToString().Trim();

        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=Demo")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/index.html"))
            .ReturnsAsync(indexContent);
        
        var layoutContent = new StringBuilder()
            .Append("<html>")
            .Append("<head>")
            .Append("<title>@PageTitle</title>")
            .Append("<link rel=\"stylesheet\" href=\"@StylePath\">")
            .Append("</head>")
            .Append("<body>")
            .Append("@Body")
            .Append("</body>")
            .Append("</html>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html"))
            .ReturnsAsync(layoutContent);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }
}