using FluentAssertions;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class FileWatcherTests
{
    private FileWatcher _instance = null!;
    private IConfiguration _configuration = null!;
    private IHtmlRenderer _htmlRenderer = null!;
    private IStyleManager _styleCompiler = null!;
    private IFileSystemService _fileSystemService = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._configuration = Substitute.For<IConfiguration>();
        this._htmlRenderer = Substitute.For<IHtmlRenderer>();
        this._styleCompiler = Substitute.For<IStyleManager>();
        this._fileSystemService = Substitute.For<IFileSystemService>();

        this._instance = new FileWatcher(
            this._configuration,
            this._htmlRenderer,
            this._styleCompiler,
            this._fileSystemService);
    }

    [TestMethod]
    public void GetOutputPathForSourceAsync_WithSimplePath_ReturnsPath()
    {
        string projectPath = "/path/to/project/src";            // /Users/larskramer/Desktop/htmlc-test/src
        string sourceFile = "/path/to/project/src/test.html";   // /Users/larskramer/Desktop/htmlc-test/src/pages.html
        string outputPath = "/path/to/project/dist";            // /Users/larskramer/Desktop/htmlc-test/dist

        string outputFile = FileWatcher.GetOutputPathForSource(sourceFile, projectPath, outputPath);

        outputFile.Should().NotBeNullOrEmpty();
        outputFile.Should().Be($"/path/to/project/dist/test.html");
    }

    [TestMethod]
    public void GetOutputPathForSourceAsync_WithSubDirectoryPath_ReturnsPath()
    {
        string projectPath = "/path/to/project/src";
        string sourceFile = "/path/to/project/src/components/test.html";
        string outputPath = "/path/to/project/dist";

        string outputFile = FileWatcher.GetOutputPathForSource(sourceFile, projectPath, outputPath);

        outputFile.Should().NotBeNullOrEmpty();
        outputFile.Should().Be($"/path/to/project/dist/components/test.html");
    }
}