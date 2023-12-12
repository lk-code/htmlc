using FluentAssertions;
using FluentDataBuilder;
using HtmlCompiler.Config;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using FluentDataBuilder.Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class TemplateManagerTests
{
    private TemplateManager _instance = null!;
    private ILogger<TemplateManager> _logger = null!;
    private IConfigurationManager _configurationManager = null!;
    private IHttpClientService _httpClientService = null!;

    [TestInitialize]
    public void SetUp()
    {
    }

    private void CreateTestInstance(IConfiguration configuration)
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        ILoggerFactory factory = serviceProvider.GetService<ILoggerFactory>()!;

        this._logger = factory.CreateLogger<TemplateManager>();

        this._configurationManager = Substitute.For<IConfigurationManager>();
        this._httpClientService = Substitute.For<IHttpClientService>();

        this._instance = new TemplateManager(
            this._logger,
            configuration,
            this._configurationManager,
            this._httpClientService);
    }

    [TestMethod]
    public async Task SearchTemplatesAsync_WithDefaultRepository_Returns()
    {
        // Arrange
        string templateName = "demo";

        IDataBuilder dataBuilder = new DataBuilder();
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        this._httpClientService.GetAsync(Arg.Any<Uri>())
            .Returns(Task.FromResult("{\"templates\":[{\"name\":\"Demo\",\"file\":\"templates/Demo.zip\"}]}"));

        // Act
        List<Template> result = (await this._instance.SearchTemplatesAsync(templateName)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(1);

        result[0].Name.Should().Be("Demo");
        result[0].FileName.Should().Be("templates/Demo.zip");
        result[0].Url.Should().Be($"https://github.com/lk-code/htmlc-templates/raw/main/templates/Demo.zip");
    }

    [TestMethod]
    public async Task SearchTemplatesAsync_WithMultipleRepositories_Returns()
    {
        // Arrange
        string templateName = "demo";

        IDataBuilder dataBuilder = new DataBuilder();
        dataBuilder.Add(TemplateManager.APPSETTINGS_KEY, new List<string>
        {
            "https://repository.com/",
            "https://storage.azure.com/test/",
            "https://test.eu/"
        });
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        this._httpClientService.GetAsync(Arg.Any<Uri>())
            .Returns(Task.FromResult("{\"templates\":[{\"name\":\"Demo\",\"file\":\"templates/Demo.zip\"}]}"));

        // Act
        List<Template> result = (await this._instance.SearchTemplatesAsync(templateName)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(4);

        result[0].Name.Should().Be("Demo");
        result[0].FileName.Should().Be("templates/Demo.zip");
    }

    [TestMethod]
    public async Task SearchTemplatesAsync_WithMultipleTemplates_Returns()
    {
        // Arrange
        string templateName = "demo";

        IDataBuilder dataBuilder = new DataBuilder();
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        this._httpClientService.GetAsync(Arg.Any<Uri>()).Returns(Task.FromResult(
            "{\"templates\":[{\"name\":\"Demo\",\"file\":\"templates/Demo.zip\"},{\"name\":\"Test\",\"file\":\"templates/Test.zip\"},{\"name\":\"Core\",\"file\":\"templates/Core.zip\"}]}"));

        // Act
        List<Template> result = (await this._instance.SearchTemplatesAsync(templateName)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(1);

        result[0].Name.Should().Be("Demo");
        result[0].FileName.Should().Be("templates/Demo.zip");
        result[0].Url.Should().Be($"https://github.com/lk-code/htmlc-templates/raw/main/templates/Demo.zip");
    }

    [TestMethod]
    public async Task SearchTemplatesAsync_WithSpecificTemplateUrl_Returns()
    {
        // Arrange
        string templateName = "https://storage.azure.com/test/templates/Demo.zip";

        IDataBuilder dataBuilder = new DataBuilder();
        dataBuilder.Add(TemplateManager.APPSETTINGS_KEY, new List<string>
        {
            "https://repository.com/",
            "https://storage.azure.com/test/",
            "https://test.eu/"
        });
        this.CreateTestInstance(dataBuilder.ToConfiguration());

        this._httpClientService.GetAsync(Arg.Any<Uri>())
            .Returns(Task.FromResult("{\"templates\":[{\"name\":\"Demo\",\"file\":\"templates/Demo.zip\"}]}"));

        // Act
        List<Template> result = (await this._instance.SearchTemplatesAsync(templateName)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(1);

        result[0].Name.Should().Be("Demo");
        result[0].FileName.Should().Be("templates/Demo.zip");
    }
}