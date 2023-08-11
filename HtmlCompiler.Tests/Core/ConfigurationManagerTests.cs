using System.Text.Json;
using HtmlCompiler.Config;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
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

        ConfigModel config = new();
        string configJson = JsonSerializer.Serialize(config);
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync(this._userConfigPath))
            .ReturnsAsync(configJson);

        this._instance.AddAsync(key, value);

        ConfigModel expectedConfig = new();
        expectedConfig.BuildBlackList = new string[] { ".png" };
        string expectedJson = JsonSerializer.Serialize(expectedConfig);
        this._fileSystemService.Verify(fs =>
            fs.FileWriteAllTextAsync(
                this._userConfigPath,
                It.Is<string>(content => content.Contains(expectedJson))
            ), Times.Once);
    }

    [TestMethod]
    public async Task RemoveAsync_WithArray_Returns()
    {
        string key = "build-blacklist";
        string value = ".png";

        ConfigModel config = new();
        config.BuildBlackList = new string[] { ".png", ".jpeg", ".svg" };
        string configJson = JsonSerializer.Serialize(config);
        this._fileSystemService.Setup(x => x.FileReadAllTextAsync(this._userConfigPath))
            .ReturnsAsync(configJson);

        this._instance.RemoveAsync(key, value);

        ConfigModel expectedConfig = new();
        expectedConfig.BuildBlackList = new string[] { ".jpeg", ".svg" };
        string expectedJson = JsonSerializer.Serialize(expectedConfig);
        this._fileSystemService.Verify(fs =>
            fs.FileWriteAllTextAsync(
                this._userConfigPath,
                It.Is<string>(content => content.Contains(expectedJson))
            ), Times.Once);
    }
}