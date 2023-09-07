using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Tests.Helper;
using NSubstitute;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class ProjectManagerTests
{
    private ProjectManager _instance = null!;
    private IFileSystemService _fileSystemService = null!;
    private IResourceLoader _resourceLoader = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileSystemService = Substitute.For<IFileSystemService>();
        this._resourceLoader = Substitute.For<IResourceLoader>();
        
        this._instance = new ProjectManager(this._fileSystemService,
            this._resourceLoader);
    }

    [TestMethod]
    public async Task AddVSCodeLiveServerConfigurationAsync_Should_Read_And_Update_File()
    {
        // Arrange
        string projectPath = "/test/path".ToSystemPath();
        string filePath = ".vscode/settings.json".ToSystemPath();
        string fullFilePath = Path.Combine(projectPath, filePath);
        string fileContent = @"{""liveServer.settings.root"": ""/src""}";

        this._fileSystemService.FileReadAllTextAsync(fullFilePath)
            .Returns(fileContent);

        // Act
        await this._instance.AddVSCodeLiveServerConfigurationAsync(projectPath);

        // Assert
        await this._fileSystemService.Received(1).FileReadAllTextAsync(fullFilePath);
        await this._fileSystemService.Received(1).FileWriteAllTextAsync(fullFilePath, Arg.Any<string>());
    }

    [TestMethod]
    public async Task AddVSCodeSupportAsync_ShouldEnsurePathAndWriteSettingsJson()
    {
        // Arrange
        string projectPath = "c:\\projects\\myproject".ToSystemPath();
        string expectedFilePath = Path.Combine(projectPath, ".vscode/settings.json".ToSystemPath());
        string? expectedVsDirectory = Path.GetDirectoryName(expectedFilePath);
        string template = "htmlc_vscode_settings_json";

        this._resourceLoader.GetResourceContentAsync($"HtmlCompiler.Core.FileTemplates.{template}.template")
            .Returns("{\"key\": \"value\"}");

        // Act
        await this._instance.AddVSCodeSupportAsync(projectPath);

        // Assert
        this._fileSystemService.Received(1).EnsurePath(expectedVsDirectory!);
        await this._fileSystemService.Received(1).FileWriteAllTextAsync(expectedFilePath, "{\"key\": \"value\"}");
    }

    [TestMethod]
    public async Task AddVSCodeSupportAsync_ShouldNotWriteSettingsJson_WhenTemplateContentIsEmpty()
    {
        // Arrange
        string projectPath = "c:\\projects\\myproject".ToSystemPath();

        this._resourceLoader.GetResourceContentAsync("unknown_template")
            .Returns(string.Empty);

        // Act
        await this._instance.AddVSCodeSupportAsync(projectPath);

        // Assert
        this._fileSystemService.Received(1).EnsurePath(Arg.Any<string>());
        await this._fileSystemService.DidNotReceive().FileWriteAllTextAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [TestMethod]
    public async Task AddDockerSupportAsync_ShouldEnsurePathAndWriteSettingsJson()
    {
        // Arrange
        string projectPath = "c:\\projects\\myproject".ToSystemPath();
        string expectedFilePath = Path.Combine(projectPath, "Dockerfile".ToSystemPath());
        string template = "htmlc_dockerfile";

        this._resourceLoader.GetResourceContentAsync($"HtmlCompiler.Core.FileTemplates.{template}.template")
            .Returns("{\"key\": \"value\"}");

        // Act
        await this._instance.AddDockerSupportAsync(projectPath);

        // Assert
        await this._fileSystemService.Received(1).FileWriteAllTextAsync(expectedFilePath, "{\"key\": \"value\"}");
    }

    [TestMethod]
    public async Task AddDockerSupportAsync_ShouldNotWriteSettingsJson_WhenTemplateContentIsEmpty()
    {
        // Arrange
        string projectPath = "c:\\projects\\myproject".ToSystemPath();

        this._resourceLoader.GetResourceContentAsync("unknown_template")
            .Returns(string.Empty);

        // Act
        await this._instance.AddDockerSupportAsync(projectPath);

        // Assert
        await this._fileSystemService.DidNotReceive().FileWriteAllTextAsync(Arg.Any<string>(), Arg.Any<string>());
    }
    
    [TestMethod]
    public async Task CreateProjectAsync_ShouldCreateFiles()
    {
        // Arrange
        string projectPath = "path/to/project";
        Dictionary<string, string> expectedFilesToCreate = new Dictionary<string, string>
        {
            { "src/index.html", "htmlc_index_html" }
        };

        foreach (var fileToCreate in expectedFilesToCreate)
        {
            string filePath = Path.Combine(projectPath, fileToCreate.Key);
            string templateKey = fileToCreate.Value;
            string templateContent = string.Empty;

            if (!string.IsNullOrEmpty(templateKey))
            {
                templateContent = "Template content for " + templateKey;
                string resourceName = $"HtmlCompiler.Core.FileTemplates.{templateKey}.template";
                this._resourceLoader.GetResourceContentAsync(resourceName)
                    .Returns(templateContent);
            }

            string? folderPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                this._fileSystemService.EnsurePath(Arg.Any<string>());
            }

            this._fileSystemService.FileWriteAllTextAsync(filePath, templateContent).Returns(Task.CompletedTask);
        }

        // Act
        await this._instance.CreateProjectAsync(projectPath);

        // Assert
        foreach (var fileToCreate in expectedFilesToCreate)
        {
            string filePath = Path.Combine(projectPath, fileToCreate.Key);
            string templateKey = fileToCreate.Value;
            string expectedTemplateContent = string.Empty;

            if (!string.IsNullOrEmpty(templateKey))
            {
                expectedTemplateContent = "Template content for " + templateKey;
            }

            string resourceName = $"HtmlCompiler.Core.FileTemplates.{templateKey}.template";
            await this._resourceLoader.Received().GetResourceContentAsync(resourceName);
            this._fileSystemService.Received().EnsurePath(Arg.Any<string>());
            await this._fileSystemService.Received().FileWriteAllTextAsync(filePath, expectedTemplateContent);

        }
    }
}