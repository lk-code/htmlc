using System.Text;
using FluentAssertions;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Moq;

namespace HtmlCompiler.Tests.Core;

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
            cssOutputFilePath,
            null);
        
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
        
        var expectedHtml = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<meta name=\"generator\" content=\"htmlc\">")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();
        
        var layoutHtml = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<body>")
            .AppendLine("@Body")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();

        var contentHtml = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("<h1>Hello World!</h1>")
            .ToString().Trim();

        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/index.html"))
            .ReturnsAsync(contentHtml);
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html"))
            .ReturnsAsync(layoutHtml);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath,
            null);
        
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
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>Demo</title>")
            .AppendLine("<meta name=\"generator\" content=\"htmlc\">")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();

        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=Demo")
            .AppendLine("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/index.html"))
            .ReturnsAsync(indexContent);
        
        var layoutContent = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>@PageTitle</title>")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("@Body")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html"))
            .ReturnsAsync(layoutContent);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath,
            null);
        
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
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>Demo</title>")
            .AppendLine("<link rel=\"stylesheet\" href=\"../../css/site.scss\">")
            .AppendLine("<meta name=\"generator\" content=\"htmlc\">")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();

        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=Demo")
            .AppendLine("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/index.html"))
            .ReturnsAsync(indexContent);
        
        var layoutContent = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>@PageTitle</title>")
            .AppendLine("<link rel=\"stylesheet\" href=\"@StylePath\">")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("@Body")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html"))
            .ReturnsAsync(layoutContent);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath,
            null);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public async Task RenderHtmlAsync_WithGlobalTag_Return()
    {
        string sourceFullFilePath = "/project/src/index.html";
        string sourceDirectory = "/project/src";
        string outputDirectory = "/project/dist";
        string? cssOutputFilePath = null;
        
        var expectedHtml = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>Demo</title>")
            .AppendLine("<meta name=\"generator\" content=\"htmlc\">")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();

        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=Demo")
            .AppendLine("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/index.html"))
            .ReturnsAsync(indexContent);
        
        var layoutContent = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>@PageTitle</title>")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("@Body")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html"))
            .ReturnsAsync(layoutContent);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath,
            null);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }
}