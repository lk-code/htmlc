using System.Text;
using FluentAssertions;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using NSubstitute;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class HtmlRendererTests
{
    private HtmlRenderer _instance = null!;
    private IFileSystemService _fileSystemService = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = Substitute.For<IFileSystemService>();
        
        this._instance = new HtmlRenderer(this._fileSystemService);
    }

    [TestMethod]
    public async Task RenderHtmlAsync_WithSimpleFileWithoutTags_Return()
    {
        string sourceFullFilePath = "/project/src/index.html";
        string sourceDirectory = "/project/src";
        string outputDirectory = "/project/dist";
        string? cssOutputFilePath = null;
        
        string expectedHtml = "Hello World!";

        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/index.html")
            .Returns("Hello World!");
        
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

        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/index.html")
            .Returns(contentHtml);
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html")
            .Returns(layoutHtml);
        
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
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/index.html")
            .Returns(indexContent);
        
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
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html")
            .Returns(layoutContent);
        
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
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/index.html")
            .Returns(indexContent);
        
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
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html")
            .Returns(layoutContent);
        
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
        IDataBuilder dataBuilder = new DataBuilder()
            .Add("Application", new DataBuilder()
                .Add("Name", "htmlc demo")
                .Add("Version", "v1.0.7"));
        
        var expectedHtml = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>htmlc demo</title>")
            .AppendLine("<meta name=\"generator\" content=\"htmlc\">")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("<pre>v1.0.7</pre>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();

        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=@Global:Application:Name")
            .AppendLine("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/index.html")
            .Returns(indexContent);
        
        var layoutContent = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>@PageTitle</title>")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("@Body")
            .AppendLine("<pre>@Global:Application:Version</pre>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html")
            .Returns(layoutContent);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath,
            dataBuilder.Build().RootElement);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public async Task RenderHtmlAsync_WithMarkdownFileTag_Return()
    {
        string sourceFullFilePath = "/project/src/index.html";
        string sourceDirectory = "/project/src";
        string outputDirectory = "/project/dist";
        string? cssOutputFilePath = null;
        IDataBuilder dataBuilder = new DataBuilder()
            .Add("Application", new DataBuilder()
                .Add("Name", "htmlc demo")
                .Add("Version", "v1.0.7"));
        
        var expectedHtml = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>htmlc demo</title>")
            .AppendLine("<meta name=\"generator\" content=\"htmlc\">")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("<section>")
            .AppendLine("<h1>Title</h1>")
            .AppendLine("<h2>Subtitle</h2>")
            .AppendLine("<p><strong>this is a bold text</strong></p>")
            .AppendLine("<p><code>this is a bold text</code></p>")
            .AppendLine("")
            .AppendLine("</section>")
            .AppendLine("<pre>v1.0.7</pre>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();

        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=@Global:Application:Name")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("<section>")
            .AppendLine("@MarkdownFile=README.md")
            .AppendLine("</section>")
            .ToString().Trim();
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/index.html")
            .Returns(indexContent);

        var markdownContent = new StringBuilder()
            .AppendLine("# Title")
            .AppendLine("## Subtitle")
            .AppendLine("")
            .AppendLine("**this is a bold text**")
            .AppendLine("")
            .AppendLine("`this is a bold text`")
            .ToString().Trim();
        this._fileSystemService.FileExists($"{sourceDirectory}/README.md")
            .Returns(true);
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/README.md")
            .Returns(markdownContent);
        
        var layoutContent = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>@PageTitle</title>")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("@Body")
            .AppendLine("<pre>@Global:Application:Version</pre>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html")
            .Returns(layoutContent);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath,
            dataBuilder.Build().RootElement);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public async Task RenderHtmlAsync_WithVariablesTag_Return()
    {
        string sourceFullFilePath = "/project/src/index.html";
        string sourceDirectory = "/project/src";
        string outputDirectory = "/project/dist";
        string? cssOutputFilePath = null;
        IDataBuilder dataBuilder = new DataBuilder()
            .Add("Application", new DataBuilder()
                .Add("Name", "htmlc demo")
                .Add("Version", "v1.0.7"));
        
        var expectedHtml = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>htmlc demo</title>")
            .AppendLine("<meta name=\"generator\" content=\"htmlc\">")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("<section>")
            .AppendLine("<p>Section Page from htmlc</p>")
            .AppendLine("</section>")
            .AppendLine("<pre>v1.0.7</pre>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();

        var indexContent = new StringBuilder()
            .AppendLine("@Var={\"Title\":\"Hello World!\"}")
            .AppendLine("@Var={\"Website\":{\"Author\":\"htmlc\"}}")
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=@Global:Application:Name")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("<section>")
            .AppendLine("@File=section.html")
            .AppendLine("</section>")
            .ToString().Trim();
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/index.html")
            .Returns(indexContent);

        var sectionContent = new StringBuilder()
            .AppendLine("@Var={\"Title\":\"Section Page\"}")
            .AppendLine("<p>@Var[\"Title\"]; from @Var[\"Website:Author\"];</p>")
            .ToString().Trim();
        this._fileSystemService.FileExists($"{sourceDirectory}/section.html")
            .Returns(true);
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/section.html")
            .Returns(sectionContent);
        
        var layoutContent = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>@PageTitle</title>")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("@Body")
            .AppendLine("<pre>@Global:Application:Version</pre>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html")
            .Returns(layoutContent);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath,
            dataBuilder.Build().RootElement);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public async Task RenderHtmlAsync_WithVariablesAndArrayTag_Return()
    {
        string sourceFullFilePath = "/project/src/index.html";
        string sourceDirectory = "/project/src";
        string outputDirectory = "/project/dist";
        string? cssOutputFilePath = null;
        IDataBuilder dataBuilder = new DataBuilder()
            .Add("Application", new DataBuilder()
                .Add("Name", "htmlc demo")
                .Add("Version", "v1.0.7"));
        
        var expectedHtml = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>htmlc demo</title>")
            .AppendLine("<meta name=\"generator\" content=\"htmlc\">")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("<section>")
            .AppendLine("<p>Section Page from htmlc</p>")
            .AppendLine("<span>this is edited by Lisa</span>")
            .AppendLine("</section>")
            .AppendLine("<pre>v1.0.7</pre>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();

        var indexContent = new StringBuilder()
            .AppendLine("@Var={\"Title\":\"Hello World!\"}")
            .AppendLine("@Var={\"Website\":{\"Author\":\"htmlc\"}}")
            .AppendLine("@Var={\"Names\":[{\"Name\":\"Max\"}, {\"Name\":\"Lisa\"}, {\"Name\":\"Fred\"}]}")
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=@Global:Application:Name")
            .AppendLine("<h1>Hello World!</h1>")
            .AppendLine("<section>")
            .AppendLine("@File=section.html")
            .AppendLine("</section>")
            .ToString().Trim();
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/index.html")
            .Returns(indexContent);

        var sectionContent = new StringBuilder()
            .AppendLine("@Var={\"Title\":\"Section Page\"}")
            .AppendLine("<p>@Var[\"Title\"]; from @Var[\"Website:Author\"];</p>")
            .AppendLine("<span>this is edited by @Var[\"Names:[1]:Name\"];</span>")
            .ToString().Trim();
        this._fileSystemService.FileExists($"{sourceDirectory}/section.html")
            .Returns(true);
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/section.html")
            .Returns(sectionContent);
        
        var layoutContent = new StringBuilder()
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendLine("<title>@PageTitle</title>")
            .AppendLine("</head>")
            .AppendLine("<body>")
            .AppendLine("@Body")
            .AppendLine("<pre>@Global:Application:Version</pre>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString().Trim();
        this._fileSystemService.FileReadAllTextAsync($"{sourceDirectory}/_layoutbase.html")
            .Returns(layoutContent);
        
        string result = await this._instance.RenderHtmlAsync(sourceFullFilePath,
            sourceDirectory,
            outputDirectory,
            cssOutputFilePath,
            dataBuilder.Build().RootElement);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }
}