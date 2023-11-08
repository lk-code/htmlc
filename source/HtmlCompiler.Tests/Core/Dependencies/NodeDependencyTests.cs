using FluentAssertions;
using HtmlCompiler.Core.Dependencies;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Dependencies;

[TestClass]
public class NodeDependencyTests
{
    private NodeDependency _instance = null!;
    private ICLIManager _cliManager = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._cliManager = Substitute.For<ICLIManager>();
        this._instance = new NodeDependency(this._cliManager);
    }

    [TestMethod]
    [DataRow("v18.1.9")]
    [DataRow("v20.4.0")]
    [DataRow("v14.17.0")]
    [DataRow("v18.1.9\n")]
    [DataRow("v20.4.0\n")]
    [DataRow("v20.9.0\n")]
    [DataRow("v18.1.9\r\n")]
    [DataRow("v20.4.0\r\n")]
    [DataRow("v20.9.0\r\n")]
    public async Task CheckAsync_WithValidVersions_Return(string consoleResult)
    {
        this._cliManager.ExecuteCommand(Arg.Any<string>())
            .Returns(consoleResult);
        
        bool result = await this._instance.CheckAsync();
        
        result.Should().BeTrue();
    }

    [TestMethod]
    [DataRow("zsh: command not found: node")]
    [DataRow("node: The term 'node' is not recognized as a name of a cmdlet, function, script file, or executable program. Check the spelling of the name, or if a path was included, verify that the path is correct and try again.")]
    public async Task CheckAsync_WithInvalidVersions_Return(string consoleResult)
    {
        this._cliManager.ExecuteCommand(Arg.Any<string>())
            .Returns(consoleResult);
        
        Func<Task> act = () => this._instance.CheckAsync();
        await act.Should().ThrowAsync<DependencyCheckFailedException>().Where(e => e.Message.Contains("Please install NodeJS"));
    }
}