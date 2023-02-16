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
}