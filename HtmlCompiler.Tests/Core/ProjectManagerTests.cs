using FluentAssertions;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Moq;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class ProjectManagerTests
{
    private ProjectManager _instance = null!;
    private Mock<IFileSystemService> _fileSystemService = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = new Mock<IFileSystemService>();
        
        this._instance = new ProjectManager(this._fileSystemService.Object);
    }

    [TestMethod]
    public async Task GetTemplateContentAsync_WithGitIgnore_Returns()
    {
        string? content = await ProjectManager.GetTemplateContentAsync("htmlc_gitignore");

        content.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task AddVSCodeLiveServerConfigurationAsync_Should_Read_And_Update_File()
    {
        // Arrange
        string projectPath = "/test/path";
        string filePath = ".vscode/settings.json";
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
}