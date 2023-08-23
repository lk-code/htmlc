using FluentAssertions;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler.Config;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Text;

namespace HtmlCompiler.Tests.Core
{
    [TestClass]
    public class TemplateManagerTests
    {
        private TemplateManager _instance = null!;
        private IConfiguration _configuration = null!;
        private IConfigurationManager _configurationManager = null!;
        private IHttpClientService _httpClientService = null!;

        [TestInitialize]
        public void SetUp()
        {
            this._configurationManager = Substitute.For<IConfigurationManager>();
            this._httpClientService = Substitute.For<IHttpClientService>();
        }

        private TemplateManager CreateTestInstance(IConfiguration configuration)
        {
            this._configurationManager = Substitute.For<IConfigurationManager>();
            this._httpClientService = Substitute.For<IHttpClientService>();

            return new TemplateManager(configuration,
                this._configurationManager,
                this._httpClientService);
        }

        [TestMethod]
        public async Task SearchTemplatesAsync_WithDefaultRepository_Returns()
        {
            // Arrange
            string templateName = "demo";
            
            IDataBuilder dataBuilder = new DataBuilder();
            
            string configJsonString = dataBuilder.Build().RootElement.GetRawText();
            using MemoryStream configJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(configJsonString));
            this._instance = this.CreateTestInstance(new ConfigurationBuilder().AddJsonStream(configJsonStream).Build());
            
            this._httpClientService.GetAsync(Arg.Any<Uri>()).Returns(Task.FromResult("{\"templates\":[{\"name\":\"Demo\",\"file\":\"templates/Demo.zip\"}]}"));

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
            
            string configJsonString = dataBuilder.Build().RootElement.GetRawText();
            using MemoryStream configJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(configJsonString));
            this._instance = this.CreateTestInstance(new ConfigurationBuilder().AddJsonStream(configJsonStream).Build());
            
            this._httpClientService.GetAsync(Arg.Any<Uri>()).Returns(Task.FromResult("{\"templates\":[{\"name\":\"Demo\",\"file\":\"templates/Demo.zip\"}]}"));

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
            
            string configJsonString = dataBuilder.Build().RootElement.GetRawText();
            using MemoryStream configJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(configJsonString));
            this._instance = this.CreateTestInstance(new ConfigurationBuilder().AddJsonStream(configJsonStream).Build());
            
            this._httpClientService.GetAsync(Arg.Any<Uri>()).Returns(Task.FromResult("{\"templates\":[{\"name\":\"Demo\",\"file\":\"templates/Demo.zip\"},{\"name\":\"Test\",\"file\":\"templates/Test.zip\"},{\"name\":\"Core\",\"file\":\"templates/Core.zip\"}]}"));

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
            
            string configJsonString = dataBuilder.Build().RootElement.GetRawText();
            using MemoryStream configJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(configJsonString));
            this._instance = this.CreateTestInstance(new ConfigurationBuilder().AddJsonStream(configJsonStream).Build());
            
            this._httpClientService.GetAsync(Arg.Any<Uri>()).Returns(Task.FromResult("{\"templates\":[{\"name\":\"Demo\",\"file\":\"templates/Demo.zip\"}]}"));

            // Act
            List<Template> result = (await this._instance.SearchTemplatesAsync(templateName)).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            
            result[0].Name.Should().Be("Demo");
            result[0].FileName.Should().Be("templates/Demo.zip");
        }
    }
}