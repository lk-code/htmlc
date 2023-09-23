using System.Text;
using HtmlCompiler.Commands;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HtmlCompiler.Tests.App.Commands;

[TestClass]
public class HtmlcCommandTests
{
    private ILogger<HtmlcCommand> _logger = null!;
    private HtmlcCommand _instance = null!;
    private IFileWatcher _fileWatcher = null!;
    private IProjectManager _projectManager = null!;
    private ITemplateManager _templateManager = null!;

    [TestInitialize]
    public void SetUp()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        ILoggerFactory factory = serviceProvider.GetService<ILoggerFactory>()!;

        this._logger = factory.CreateLogger<HtmlcCommand>();
        
        this._fileWatcher = Substitute.For<IFileWatcher>();
        this._projectManager = Substitute.For<IProjectManager>();
        this._templateManager = Substitute.For<ITemplateManager>();

        this._instance = new HtmlcCommand(this._logger,
            this._fileWatcher,
            this._projectManager,
            this._templateManager);
    }

    [TestMethod]
    public async Task New_WithTemplateNameAndNoResult_DisplayError()
    {
        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=Demo")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._templateManager.SearchTemplatesAsync(Arg.Any<string>())
            .Returns(Task.FromResult<IEnumerable<Template>>(new List<Template>()));

        await this._instance.New(false, false, false, "DemoTemplate");
    }

    [TestMethod]
    public async Task New_WithTemplateNameAndMultipleResults_DisplayError()
    {
        var indexContent = new StringBuilder()
            .AppendLine("@Layout=_layoutbase.html")
            .AppendLine("@PageTitle=Demo")
            .Append("<h1>Hello World!</h1>")
            .ToString().Trim();
        this._templateManager.SearchTemplatesAsync(Arg.Any<string>())
            .Returns(new List<Template>
            {
                new Template("DemoTemplate", "demo.zip", "https://example.com/demo.zip"),
                new Template("DemoTemplate", "demo.zip", "https://another-repository.com/demo.zip")
            });

        await this._instance.New(false, false, false, "DemoTemplate");
    }
}