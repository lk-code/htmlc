using System.Text.Json;
using HtmlCompiler.Config;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using NSubstitute;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class ConfigurationManagerTests
{
    private string _userConfigPath = "/.htmlc";
    private ConfigurationManager _instance = null!;
    private IFileSystemService _fileSystemService = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = Substitute.For<IFileSystemService>();

        this._instance = new ConfigurationManager(this._userConfigPath,
            this._fileSystemService);
    }

    [TestMethod]
    public async Task AddAsync_WithArray_Returns()
    {
        string key = "build-blacklist";
        string value = ".png";

        ConfigModel config = new();
        string configJson = JsonSerializer.Serialize(config);
        this._fileSystemService.FileReadAllTextAsync(this._userConfigPath)
            .Returns(configJson);

        this._instance.AddAsync(key, value);

        ConfigModel expectedConfig = new();
        expectedConfig.BuildBlackList = new string[] { ".png" };
        string expectedJson = JsonSerializer.Serialize(expectedConfig);
        await this._fileSystemService.Received(1).FileWriteAllTextAsync(
            this._userConfigPath,
            Arg.Is<string>(content => content.Contains(expectedJson))
        );

    }

    [TestMethod]
    public async Task RemoveAsync_WithArray_Returns()
    {
        string key = "build-blacklist";
        string value = ".png";

        ConfigModel config = new();
        config.BuildBlackList = new string[] { ".png", ".jpeg", ".svg" };
        string configJson = JsonSerializer.Serialize(config);
        this._fileSystemService.FileReadAllTextAsync(this._userConfigPath)
            .Returns(configJson);

        this._instance.RemoveAsync(key, value);

        ConfigModel expectedConfig = new();
        expectedConfig.BuildBlackList = new string[] { ".jpeg", ".svg" };
        string expectedJson = JsonSerializer.Serialize(expectedConfig);
        await this._fileSystemService.Received(1).FileWriteAllTextAsync(
            this._userConfigPath,
            Arg.Is<string>(content => content.Contains(expectedJson))
        );

    }
}