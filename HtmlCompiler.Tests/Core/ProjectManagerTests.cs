using FluentAssertions;
using HtmlCompiler.Core;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class ProjectManagerTests
{
    [TestMethod]
    public async Task GetTemplateContentAsync_WithGitIgnore_Returns()
    {
        ProjectManager projectManager = new ProjectManager();
        
        string? content = await projectManager.GetTemplateContentAsync("htmlc_gitignore");

        content.Should().NotBeNullOrEmpty();
    }
}