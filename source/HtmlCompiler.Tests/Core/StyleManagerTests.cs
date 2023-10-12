using FluentAssertions;
using FluentDataBuilder;
using FluentDataBuilder.Microsoft.Extensions.Configuration;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class StyleManagerTests
{
    private StyleManager _instance = null!;
    private IFileSystemService _fileSystemService = null!;
    private ICLIManager _cliManager = null!;
    
    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = Substitute.For<IFileSystemService>();
        this._cliManager = Substitute.For<ICLIManager>();
    }

    private void CreateTestInstance(IConfiguration configuration)
    {
        this._instance = new StyleManager(configuration,
            this._fileSystemService,
            this._cliManager);
    }

    [TestMethod]
    public async Task CompileStyleAsync_WithNonExistingStyleCommand_Throws()
    {
        string sourceDirectoryPath = "/Users/larskramer/htmlc-test/src";
        string outputDirectoryPath = "/Users/larskramer/htmlc-test/dist";
        string? styleSourceFilePath = "/Users/larskramer/htmlc-test/src/styles/main.scss";

        IDataBuilder dataBuilder = new DataBuilder();
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        this._fileSystemService.FileExists(styleSourceFilePath).Returns(true);
        
        Func<Task> act = () => this._instance.CompileStyleAsync(sourceDirectoryPath, outputDirectoryPath, styleSourceFilePath);
        await act.Should().ThrowAsync<StyleCommandNotFoundException>().Where(e => e.Message.Contains($"style compile command for 'scss' not found"));
    }

    [TestMethod]
    public async Task CompileStyleAsync_WithoutStyleSource_Returns()
    {
        string sourceDirectoryPath = "/Users/larskramer/htmlc-test/src";
        string outputDirectoryPath = "/Users/larskramer/htmlc-test/dist";
        string? styleSourceFilePath = null;

        IDataBuilder dataBuilder = new DataBuilder();
        this.CreateTestInstance(dataBuilder.ToConfiguration());
        
        string? result = await this._instance.CompileStyleAsync(sourceDirectoryPath, outputDirectoryPath, styleSourceFilePath);
        
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task CompileStyleAsync_WithStyleSourceNotExisting_Throws()
    {
        string sourceDirectoryPath = "/Users/larskramer/htmlc-test/src";
        string outputDirectoryPath = "/Users/larskramer/htmlc-test/dist";
        string? styleSourceFilePath = "/Users/larskramer/htmlc-test/src/styles/main.scss";

        IDataBuilder dataBuilder = new DataBuilder();
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        this._fileSystemService.FileExists(styleSourceFilePath).Returns(false);
        
        Func<Task> act = () => this._instance.CompileStyleAsync(sourceDirectoryPath, outputDirectoryPath, styleSourceFilePath);
        await act.Should().ThrowAsync<StyleNotFoundException>();
    }

    [TestMethod]
    public async Task CompileStyleAsync_WithStyleSourceWithNotSupportedType_Throws()
    {
        string sourceDirectoryPath = "/Users/larskramer/htmlc-test/src";
        string outputDirectoryPath = "/Users/larskramer/htmlc-test/dist";
        string? styleSourceFilePath = "/Users/larskramer/htmlc-test/src/styles/main.scss";

        IDataBuilder dataBuilder = new DataBuilder();
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        this._fileSystemService.FileExists(styleSourceFilePath)
            .Returns(true);
        
        Func<Task> act = () => this._instance.CompileStyleAsync(sourceDirectoryPath, outputDirectoryPath, styleSourceFilePath);
        await act.Should().ThrowAsync<StyleCommandNotFoundException>();
    }

    [TestMethod]
    public async Task CompileStyleAsync_WithStyleSourceWithExisting_Returns()
    {
        string sourceDirectoryPath = "/Users/larskramer/htmlc-test/src";
        string outputDirectoryPath = "/Users/larskramer/htmlc-test/dist";
        string? styleSourceFilePath = "/Users/larskramer/htmlc-test/src/styles/main.scss";

        IDataBuilder dataBuilder = new DataBuilder()
            .Add("style-commands", new DataBuilder()
                .Add("scss", "sass {0} {1}")
                .Add("sass", "sass {0} {1}")
                .Add("less", "less {0} {1}"));
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        this._fileSystemService.FileExists(styleSourceFilePath)
            .Returns(true);
        
        string? result = await this._instance.CompileStyleAsync(sourceDirectoryPath, outputDirectoryPath, styleSourceFilePath);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be("/Users/larskramer/htmlc-test/dist/styles/main.css");
    }
}