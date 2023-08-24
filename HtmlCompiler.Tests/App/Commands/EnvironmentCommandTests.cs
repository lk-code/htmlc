using System.Text;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler.Commands;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Tests.App.Commands;

[TestClass]
public class EnvironmentCommandTests
{
    private EnvironmentCommand _instance = null!;

    [TestInitialize]
    public void SetUp()
    {
    }

    private EnvironmentCommand CreateTestInstance(IConfiguration configuration)
    {
        return new EnvironmentCommand(configuration);
    }

    [TestMethod]
    public async Task New_WithTemplateNameAndNoResult_DisplayError()
    {
        IDataBuilder dataBuilder = new DataBuilder();
        string configJsonString = dataBuilder.Build().RootElement.GetRawText();
        using MemoryStream configJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(configJsonString));
        this._instance = this.CreateTestInstance(new ConfigurationBuilder().AddJsonStream(configJsonStream).Build());

        this._instance.Check();
    }
}