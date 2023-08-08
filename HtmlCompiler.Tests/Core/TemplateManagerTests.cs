using FluentAssertions;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler.Config;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Configuration;
using Moq;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class TemplateManagerTests
{
    private TemplateManager _instance = null!;
    private IConfiguration _configuration = null!;
    private Mock<IConfigurationManager> _configurationManager = null!;

    [TestInitialize]
    public void SetUp()
    {
        IDataBuilder dataBuilder = new DataBuilder();

        string configJsonString = dataBuilder.Build().RootElement.GetRawText();
        using MemoryStream configJsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(configJsonString));
        this._configuration = new ConfigurationBuilder().AddJsonStream(configJsonStream).Build();
        
        this._configurationManager = new Mock<IConfigurationManager>();
        
        this._instance = new TemplateManager(this._configuration,
            this._configurationManager.Object);
    }

    [TestMethod]
    public async Task SearchTemplatesAsync_WithDefault_Returns()
    {
        // Arrange
        string templateName = "demo";

        // Act
        IEnumerable<Template> result = await this._instance.SearchTemplatesAsync(templateName);

        // Assert
        result.Should().NotBeNull();
    }
}