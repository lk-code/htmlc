using FluentAssertions;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Tests.Helper;
using Moq;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class ProjectManagerTests
{
    private ProjectManager _instance = null!;
    private Mock<IFileSystemService> _fileSystemService = null!;
    private Mock<IResourceLoader> _resourceLoader = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = new Mock<IFileSystemService>();
        this._resourceLoader = new Mock<IResourceLoader>();
        
        this._instance = new ProjectManager(this._fileSystemService.Object,
            this._resourceLoader.Object);
    }

    [TestMethod]
    public async Task AddVSCodeLiveServerConfigurationAsync_Should_Read_And_Update_File()
    {
        // Arrange
        string projectPath = "/test/path".ToSystemPath();
        string filePath = ".vscode/settings.json".ToSystemPath();
        string fullFilePath = Path.Combine(projectPath, filePath);
        string fileContent = @"{""liveServer.settings.root"": ""/src""}";

        this._fileSystemService.Setup(x => x.FileReadAllTextAsync(fullFilePath))
            .ReturnsAsync(fileContent);

        // Act
        await this._instance.AddVSCodeLiveServerConfigurationAsync(projectPath);

        // Assert
        this._fileSystemService.Verify(x => x.FileReadAllTextAsync(fullFilePath), Times.Once);
        this._fileSystemService.Verify(x => x.FileWriteAllTextAsync(fullFilePath, It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task AddVSCodeSupportAsync_ShouldEnsurePathAndWriteSettingsJson()
    {
        // Arrange
        var projectPath = "c:\\projects\\myproject".ToSystemPath();
        var expectedFilePath = Path.Combine(projectPath, ".vscode/settings.json".ToSystemPath());
        var expectedVsDirectory = Path.GetDirectoryName(expectedFilePath);
        var template = "htmlc_vscode_settings_json";

        this._resourceLoader.Setup(r => r.GetResourceContentAsync($"HtmlCompiler.Core.FileTemplates.{template}.template"))
            .ReturnsAsync("{\"key\": \"value\"}");

        // Act
        await this._instance.AddVSCodeSupportAsync(projectPath);

        // Assert
        this._fileSystemService.Verify(fs => fs.EnsurePath(expectedVsDirectory), Times.Once);
        this._fileSystemService.Verify(fs => fs.FileWriteAllTextAsync(expectedFilePath, "{\"key\": \"value\"}"), Times.Once);
    }

    [TestMethod]
    public async Task AddVSCodeSupportAsync_ShouldNotWriteSettingsJson_WhenTemplateContentIsEmpty()
    {
        // Arrange
        var projectPath = "c:\\projects\\myproject".ToSystemPath();

        this._resourceLoader.Setup(r => r.GetResourceContentAsync("unknown_template"))
            .ReturnsAsync(string.Empty);

        // Act
        await this._instance.AddVSCodeSupportAsync(projectPath);

        // Assert
        this._fileSystemService.Verify(fs => fs.EnsurePath(It.IsAny<string>()), Times.Once);
        this._fileSystemService.Verify(fs => fs.FileWriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task AddDockerSupportAsync_ShouldEnsurePathAndWriteSettingsJson()
    {
        // Arrange
        var projectPath = "c:\\projects\\myproject".ToSystemPath();
        var expectedFilePath = Path.Combine(projectPath, "Dockerfile".ToSystemPath());
        var template = "htmlc_dockerfile";

        this._resourceLoader.Setup(r => r.GetResourceContentAsync($"HtmlCompiler.Core.FileTemplates.{template}.template"))
            .ReturnsAsync("{\"key\": \"value\"}");

        // Act
        await this._instance.AddDockerSupportAsync(projectPath);

        // Assert
        this._fileSystemService.Verify(fs => fs.FileWriteAllTextAsync(expectedFilePath, "{\"key\": \"value\"}"), Times.Once);
    }

    [TestMethod]
    public async Task AddDockerSupportAsync_ShouldNotWriteSettingsJson_WhenTemplateContentIsEmpty()
    {
        // Arrange
        var projectPath = "c:\\projects\\myproject".ToSystemPath();

        this._resourceLoader.Setup(r => r.GetResourceContentAsync("unknown_template"))
            .ReturnsAsync(string.Empty);

        // Act
        await this._instance.AddDockerSupportAsync(projectPath);

        // Assert
        this._fileSystemService.Verify(fs => fs.FileWriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}