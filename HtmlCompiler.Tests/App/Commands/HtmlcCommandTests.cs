using System.Text;
using HtmlCompiler.Commands;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace HtmlCompiler.Tests.App.Commands;

[TestClass]
public class HtmlcCommandTests
{
    private HtmlcCommand _instance = null!;
    private Mock<IFileWatcher> _fileWatcher = null!;
    private Mock<IProjectManager> _projectManager = null!;
    private Mock<ITemplateManager> _templateManager = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._fileWatcher = new Mock<IFileWatcher>();
        this._projectManager = new Mock<IProjectManager>();
        this._templateManager = new Mock<ITemplateManager>();

        this._instance = new HtmlcCommand(this._fileWatcher.Object,
            this._projectManager.Object,
            this._templateManager.Object);
    }

    [TestMethod]
    public async Task New_WithTemplateNameAndNoResult_DisplayError()
    {
        string key = "";
        string action = "";
        string value = "";

        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=Demo")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._templateManager.Setup(x => x.SearchTemplatesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<Template>
            {
            });

        await this._instance.New(false, false, false, "DemoTemplate");
        
        // this._logger.Verify(fs => fs.LogError("No templates found."), Times.Once);
    }

    [TestMethod]
    public async Task New_WithTemplateNameAndMultipleResults_DisplayError()
    {
        string key = "";
        string action = "";
        string value = "";

        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=Demo")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._templateManager.Setup(x => x.SearchTemplatesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<Template>
            {
                new() { Name = "DemoTemplate", FileName = "demo.zip", Url = "https://example.com/demo.zip" },
                new() { Name = "DemoTemplate", FileName = "demo.zip", Url = "https://another-repository.com/demo.zip" },
            });

        await this._instance.New(false, false, false, "DemoTemplate");
        
        // this._logger.Verify(fs => fs.LogError("Multiple templates found. Please specify the template name."), Times.Once);
    }
}