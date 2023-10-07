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
        
        _instance = new TemplatePackagingService(
            this._logger,
            dataBuilder.ToConfiguration(),
            this._fileSystemService
        );
    }

    [TestMethod]
    public async Task CreateAsync_WithEmptyPaths_Throws()
    {
        string sourcePath = "";
        string outputPath = "";
        
        Func<Task> act = () => this._instance.CreateAsync(sourcePath, outputPath);
        await act.Should().ThrowAsync<ArgumentException>().Where(e => e.Message.Contains("invalid source path"));
    }

    [TestMethod]
    public async Task CreateAsync_WithValidSourceAndEmptyOutputPath_Returns()
    {
        string sourcePath = "/project";
        string outputPath = "";

        await this._fileSystemService.FileWriteAllTextAsync("/project/src/template.zip", Arg.Any<string>());
        
        await this._instance.CreateAsync(sourcePath, outputPath);

        await this._fileSystemService.Received(1)
            .FileWriteAllTextAsync("/project/src/template.zip", Arg.Any<string>());
    }

    [TestMethod]
    public async Task CreateAsync_WithValidPaths_Returns()
    {
        string sourcePath = "/project";
        string outputPath = "/dist/template.zip";

        await this._fileSystemService.FileWriteAllTextAsync("/dist/template.zip", Arg.Any<string>());
        
        await this._instance.CreateAsync(sourcePath, outputPath);

        await this._fileSystemService.Received(1)
            .FileWriteAllTextAsync("/dist/template.zip", Arg.Any<string>());
    }

    [TestMethod]
    public async Task CreateAsync_WithInvalidOutputFileType_Throws()
    {
        string sourcePath = "/project";
        string outputPath = "/dist/template.rar";
        
        Func<Task> act = () => this._instance.CreateAsync(sourcePath, outputPath);
        await act.Should().ThrowAsync<ArgumentException>().Where(e => e.Message.Contains("invalid filetype or no filename given."));
    }
}