using FluentAssertions;
using HtmlCompiler.Core.Dependencies;
using HtmlCompiler.Core.Interfaces;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Dependencies;

[TestClass]
public class LessDependencyTests
{
    private LessDependency _instance = null!;
    private ICLIManager _cliManager = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._cliManager = Substitute.For<ICLIManager>();
        this._instance = new LessDependency(this._cliManager);
    }

    [TestMethod]
    [DataRow("less 581.2 (POSIX regular expressions)")]
    public async Task CheckAsync_WithValidVersions_Return(string consoleResult)
    {
        this._cliManager.ExecuteCommand(Arg.Any<string>())
            .Returns(consoleResult);
        
        bool result = await this._instance.CheckAsync();
        
        result.Should().BeTrue();
    }

    [TestMethod]
    [DataRow("bash: less: command not found")]
    [DataRow("less: The term 'less' is not recognized as a name of a cmdlet, function, script file, or executable program. Check the spelling of the name, or if a path was included, verify that the path is correct and try again.")]
    public async Task CheckAsync_WithInvalidVersions_Return(string consoleResult)
    {
        this._cliManager.ExecuteCommand(Arg.Any<string>())
            .Returns(consoleResult);
        
        bool result = await this._instance.CheckAsync();
        
        result.Should().BeFalse();
    }
}