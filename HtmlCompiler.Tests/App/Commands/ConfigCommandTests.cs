using HtmlCompiler.Commands;
using HtmlCompiler.Config;
using Moq;

namespace HtmlCompiler.Tests.App.Commands;

[TestClass]
public class ConfigCommandTests
{
    private ConfigCommand _instance = null!;
    private Mock<IConfigurationManager> _configurationManager = null!;
    
    [TestInitialize]
    public void SetUp()
    {
        this._configurationManager = new Mock<IConfigurationManager>();

        this._instance = new ConfigCommand(this._configurationManager.Object);
    }

    [TestMethod]
    public async Task Config_WithUnknownAction_Throws()
    {
        string key = "";
        string action = "";
        string value = "";

        Assert.ThrowsExceptionAsync<ArgumentException>(() => this._instance.Config(key, action, value));
    }
}