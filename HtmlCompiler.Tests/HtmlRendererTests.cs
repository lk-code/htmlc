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
    public void RenderHtmlEscapeBlocks_WithSimpleHtml_Returns()
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

        string result = HtmlRenderer.RenderHtmlEscapeBlocks(html);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public void RenderHtmlEscapeBlocks_WithMultipleTags_Returns()
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

        string result = HtmlRenderer.RenderHtmlEscapeBlocks(html);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
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

    // [TestMethod]
    // public async Task RenderHtmlAsync_WithMultipleFiles_Return()
    // {
    //     string sourceFullFilePath = "/project/src/index.html";
    //     string sourceDirectory = "/project/src";
    //     string outputDirectory = "/project/dist";
    //     string? cssOutputFilePath = null;
    //     
    //     var expectedHtml = new StringBuilder()
    //         .Append("<html>")
    //         .Append("<head>")
    //         .Append("<title>Demo</title>")
    //         .Append("<meta name=\"generator\" content=\"htmlc\">")
    //         .Append("</head>")
    //         .Append("<body>")
    //         .Append("<h1>Hello World!</h1>")
    //         .Append("<section>")
    //         .Append("<footer>")
    //         .Append("<div>")
    //         .Append("<p>Demo</p>")
    //         .Append("<p>a footer value</p>")
    //         .Append("</div>")
    //         .AppendLine("</footer>")
    //         .Append("</section>")
    //         .Append("</body>")
    //         .Append("</html>")
    //         .ToString().Trim();
    //
    //     var indexContent = new StringBuilder()
    //         .AppendLine("@Layout=_layoutbase.html")
    //         .AppendLine("@PageTitle=Demo")
    //         .Append("<h1>Hello World!</h1>")
    //         .ToString().Trim();
    //     this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/index.html"))
    //         .ReturnsAsync(indexContent);
    //     
    //     var footerContent = new StringBuilder()
    //         .Append("<footer>")
    //         .Append("<div>")
    //         .Append("<p>@PageTitle</p>")
    //         .Append("<p>a footer value</p>")
    //         .Append("</div>")
    //         .Append("</footer>")
    //         .ToString().Trim();
    //     this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/_footer.html"))
    //         .ReturnsAsync(footerContent);
    //     
    //     var layoutContent = new StringBuilder()
    //         .Append("<html>")
    //         .Append("<head>")
    //         .Append("<title>@PageTitle</title>")
    //         .Append("</head>")
    //         .Append("<body>")
    //         .Append("@Body")
    //         .Append("<section>")
    //         .AppendLine("@File=_footer.html")
    //         .Append("</section>")
    //         .Append("</body>")
    //         .Append("</html>")
    //         .ToString().Trim();
    //     this._fileSystemService.Setup(x => x.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html"))
    //         .ReturnsAsync(layoutContent);
    //     
    //     string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
    //         sourceDirectory,
    //         outputDirectory,
    //         cssOutputFilePath);
    //     
    //     result.Should().NotBeNullOrEmpty();
    //     result.Should().Be(expectedHtml);
    // }
}