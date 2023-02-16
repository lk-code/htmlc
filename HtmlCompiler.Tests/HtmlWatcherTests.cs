using FluentAssertions;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace HtmlCompiler.Tests;

[TestClass]
public class HtmlWatcherTests
{
    private HtmlWatcher _instance = null!;
    private Mock<IConfiguration> _configuration = null!;
    private Mock<IHtmlRenderer> _htmlRenderer = null!;
    private Mock<IStyleCompiler> _styleCompiler = null!;
    private Mock<IFileSystemService> _fileSystemService = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._configuration = new Mock<IConfiguration>();
        this._htmlRenderer = new Mock<IHtmlRenderer>();
        this._styleCompiler = new Mock<IStyleCompiler>();
        this._fileSystemService = new Mock<IFileSystemService>();

        this._instance = new HtmlWatcher(
            this._configuration.Object,
            this._htmlRenderer.Object,
            this._styleCompiler.Object,
            this._fileSystemService.Object);
    }

    [TestMethod]
    public void GetOutputPathForSourceAsync_WithSimplePath_ReturnsPath()
    {
        string projectPath = "/path/to/project/src";            // /Users/larskramer/Desktop/htmlc-test/src
        string sourceFile = "/path/to/project/src/test.html";   // /Users/larskramer/Desktop/htmlc-test/src/pages.html
        string outputPath = "/path/to/project/dist";            // /Users/larskramer/Desktop/htmlc-test/dist

        string outputFile = this._instance.GetOutputPathForSource(sourceFile, projectPath, outputPath);

        outputFile.Should().NotBeNullOrEmpty();
        outputFile.Should().Be($"/path/to/project/dist/test.html");
    }

    [TestMethod]
    public void GetOutputPathForSourceAsync_WithSubDirectoryPath_ReturnsPath()
    {
        string projectPath = "/path/to/project/src";
        string sourceFile = "/path/to/project/src/components/test.html";
        string outputPath = "/path/to/project/dist";

        string outputFile = this._instance.GetOutputPathForSource(sourceFile, projectPath, outputPath);

        outputFile.Should().NotBeNullOrEmpty();
        outputFile.Should().Be($"/path/to/project/dist/components/test.html");
    }
}