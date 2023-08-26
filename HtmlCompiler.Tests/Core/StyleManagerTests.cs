using System.Text;
using FluentAssertions;
using FluentDataBuilder;
using FluentDataBuilder.Json;
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
    
    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = Substitute.For<IFileSystemService>();
    }

    private void CreateTestInstance(IConfiguration configuration)
    {
        this._instance = new StyleManager(configuration, this._fileSystemService);
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

        this._fileSystemService.FileExists(styleSourceFilePath).Returns(false);
        
        Func<Task> act = () => this._instance.CompileStyleAsync(sourceDirectoryPath, outputDirectoryPath, styleSourceFilePath);
        act.Should().ThrowAsync<StyleNotFoundException>().Where(e => e.Message.Contains($"style file not found at {styleSourceFilePath}"));
    }

    [TestMethod]
    public async Task CompileStyleAsync_WithStyleSourceWithNotSupportedType_Throws()
    {
        string sourceDirectoryPath = "/Users/larskramer/htmlc-test/src";
        string outputDirectoryPath = "/Users/larskramer/htmlc-test/dist";
        string? styleSourceFilePath = "/Users/larskramer/htmlc-test/src/styles/main.scss";

        this._fileSystemService.FileExists(styleSourceFilePath).Returns(true);
        
        Func<Task> act = () => this._instance.CompileStyleAsync(sourceDirectoryPath, outputDirectoryPath, styleSourceFilePath);
        act.Should().ThrowAsync<UnsupportedStyleTypeException>().Where(e => e.Message.Contains($"style file not found at {styleSourceFilePath}"));
    }

    [TestMethod]
    public async Task CompileStyleAsync_WithStyleSourceWithExisting_Returns()
    {
        string sourceDirectoryPath = "/Users/larskramer/htmlc-test/src";
        string outputDirectoryPath = "/Users/larskramer/htmlc-test/dist";
        string? styleSourceFilePath = "/Users/larskramer/htmlc-test/src/styles/main.scss";

        this._fileSystemService.FileExists(styleSourceFilePath).Returns(true);
        
        string? result = await this._instance.CompileStyleAsync(sourceDirectoryPath, outputDirectoryPath, styleSourceFilePath);

        result.Should().NotBeNullOrEmpty();
    }
}