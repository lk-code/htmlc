using System.Text;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler.Config;
using HtmlCompiler.Core.Interfaces;
using Moq;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class ConfigurationManagerTests
{
    private string _userConfigPath = "/.htmlc";
    private ConfigurationManager _instance = null!;
    private Mock<IFileSystemService> _fileSystemService = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = new Mock<IFileSystemService>();

        this._instance = new ConfigurationManager(this._userConfigPath, this._fileSystemService.Object);
    }

    [TestMethod]
    public async Task AddAsync_WithArray_Returns()
    {
        string key = "build-blacklist";
        string value = ".png";

        string htmlConfig = new DataBuilder()
            .Build()
            .RootElement.GetRawText();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync(this._userConfigPath))
            .ReturnsAsync(htmlConfig);

        this._instance.AddAsync(key, value);

        string expectedConfig = new DataBuilder()
            .Add("build-blacklist", new string[] { ".png" })
            .Build()
            .RootElement.GetRawText();
        this._fileSystemService.Verify(fs =>
            fs.FileWriteAllTextAsync(
                this._userConfigPath,
                It.Is<string>(content => content.Contains("{\"build-blacklist\":[\".png\"]}"))
            ), Times.Once);
    }

    [TestMethod]
    public async Task RemoveAsync_WithArray_Returns()
    {
        string key = "build-blacklist";
        string value = ".png";

        string htmlConfig = new DataBuilder()
            .Add("build-blacklist", new string[] { ".png", ".jpeg", ".svg" })
            .Build()
            .RootElement.GetRawText();
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync(this._userConfigPath))
            .ReturnsAsync(htmlConfig);

        this._instance.RemoveAsync(key, value);

        string expectedConfig = new DataBuilder()
            .Add("build-blacklist", new string[] { ".jpeg", ".svg" })
            .Build()
            .RootElement.GetRawText();
        this._fileSystemService.Verify(fs =>
            fs.FileWriteAllTextAsync(
                this._userConfigPath,
                It.Is<string>(content => content.Contains(expectedConfig))
            ), Times.Once);
    }
}