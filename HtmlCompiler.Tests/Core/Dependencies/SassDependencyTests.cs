using FluentAssertions;
using HtmlCompiler.Core.Dependencies;
using HtmlCompiler.Core.Interfaces;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Dependencies;

[TestClass]
public class SassDependencyTests
{
    private SassDependency _instance = null!;
    private ICLIManager _cliManager = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._cliManager = Substitute.For<ICLIManager>();
        this._instance = new SassDependency(this._cliManager);
    }

    [TestMethod]
    [DataRow("1.66.1 compiled with dart2js 3.1.0")]
    public async Task CheckAsync_WithValidVersions_Return(string consoleResult)
    {
        this._cliManager.ExecuteCommand(Arg.Any<string>())
            .Returns(consoleResult);
        
        bool result = await this._instance.CheckAsync();
        
        result.Should().BeTrue();
    }

    [TestMethod]
    [DataRow("bash: sass: command not found")]
    [DataRow("sass: The term 'sass' is not recognized as a name of a cmdlet, function, script file, or executable program. Check the spelling of the name, or if a path was included, verify that the path is correct and try again.")]
    public async Task CheckAsync_WithInvalidVersions_Return(string consoleResult)
    {
        this._cliManager.ExecuteCommand(Arg.Any<string>())
            .Returns(consoleResult);
        
        bool result = await this._instance.CheckAsync();
        
        result.Should().BeFalse();
    }
}