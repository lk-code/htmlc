using FluentAssertions;
using FluentDataBuilder;
using FluentDataBuilder.Microsoft.Extensions.Configuration;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class TemplatePackagingServiceTests
{
    private TemplatePackagingService _instance = null!;
    private ILogger<TemplatePackagingService> _logger = null!;
    private IFileSystemService _fileSystemService = null!;
    private IZipArchiveProvider _zipArchiveProvider = null!;

    [TestInitialize]
    public void SetUp()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        ILoggerFactory factory = serviceProvider.GetService<ILoggerFactory>()!;

        this._logger = factory.CreateLogger<TemplatePackagingService>();

        IDataBuilder dataBuilder = new DataBuilder();
        this._fileSystemService = Substitute.For<IFileSystemService>();
        this._zipArchiveProvider = Substitute.For<IZipArchiveProvider>();

        _instance = new TemplatePackagingService(
            this._logger,
            dataBuilder.ToConfiguration(),
            this._fileSystemService,
            this._zipArchiveProvider
        );
    }

    [TestMethod]
    public async Task CreateAsync_WithEmptyPaths_Throws()
    {
        string sourcePath = "";
        string outputPath = "";

        Func<Task> act = () => this._instance.CreateAsync(sourcePath, outputPath);
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .Where(e => e.Message.Contains("invalid source path"));
    }

    [TestMethod]
    public async Task CreateAsync_WithExistingTemplateArchive_Returns()
    {
        string sourcePath = "/project";
        string outputPath = "";

        this._fileSystemService.GetAllFiles("/project/src")
            .Returns(new List<string>());
        this._fileSystemService.FileExists("/project/template.zip")
            .Returns(true);
        this._fileSystemService.Delete("/project/template.zip")
            .Returns(true);
        this._zipArchiveProvider.CreateZipFile(Arg.Any<IEnumerable<string>>(),
                "/project/src",
                "/project/template.zip")
            .Returns(new List<string>());

        await this._instance.CreateAsync(sourcePath, outputPath);

        this._fileSystemService.Received(1)
            .GetAllFiles("/project/src");
        this._fileSystemService.Received(1)
            .FileExists("/project/template.zip");
        this._fileSystemService.Received(1)
            .Delete("/project/template.zip");
    }

    [TestMethod]
    public async Task CreateAsync_WithValidSourceAndEmptyOutputPath_Returns()
    {
        string sourcePath = "/project";
        string outputPath = "";

        this._fileSystemService.GetAllFiles("/project/src")
            .Returns(new List<string>());
        this._fileSystemService.FileExists("/project/template.zip")
            .Returns(false);
        this._zipArchiveProvider.CreateZipFile(Arg.Any<IEnumerable<string>>(),
                "/project/src",
                "/project/template.zip")
            .Returns(new List<string>());

        await this._instance.CreateAsync(sourcePath, outputPath);

        this._fileSystemService.Received(1)
            .GetAllFiles("/project/src");
        this._fileSystemService.Received(1)
            .FileExists("/project/template.zip");
        this._zipArchiveProvider.Received(1)
            .CreateZipFile(Arg.Any<List<string>>(),
                "/project/src",
                "/project/template.zip");
    }

    [TestMethod]
    public async Task CreateAsync_WithValidPaths_Returns()
    {
        string sourcePath = "/project";
        string outputPath = "/dist/template.zip";

        this._fileSystemService.GetAllFiles("/project/src")
            .Returns(new List<string>());
        this._fileSystemService.FileExists("/dist/template.zip")
            .Returns(false);
        this._zipArchiveProvider.CreateZipFile(Arg.Any<IEnumerable<string>>(),
                "/project/src",
                "/dist/template.zip")
            .Returns(new List<string>());

        await this._instance.CreateAsync(sourcePath, outputPath);

        this._fileSystemService.Received(1)
            .GetAllFiles("/project/src");
        this._fileSystemService.Received(1)
            .FileExists("/dist/template.zip");
        this._zipArchiveProvider.Received(1)
            .CreateZipFile(Arg.Any<List<string>>(),
                "/project/src",
                "/dist/template.zip");
    }

    [TestMethod]
    public async Task CreateAsync_WithInvalidOutputFileType_Throws()
    {
        string sourcePath = "/project";
        string outputPath = "/dist/template.rar";

        Func<Task> act = () => this._instance.CreateAsync(sourcePath, outputPath);
        await act.Should().ThrowAsync<ArgumentException>()
            .Where(e => e.Message.Contains("invalid filetype or no filename given."));
    }
}