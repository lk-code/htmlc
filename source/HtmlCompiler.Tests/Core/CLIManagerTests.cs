using FluentAssertions;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Exceptions;

namespace HtmlCompiler.Tests.Core;

[TestClass]
public class CLIManagerTests
{
    private CLIManager _instance = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._instance = new CLIManager();
    }

    [TestMethod]
    public void ExecuteCommand_WithDotnetVersion_Returns()
    {
        string command = "dotnet --version";

        string result = _instance.ExecuteCommand(command);
        
        result.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public void ExecuteCommand_WithInvalidCommandDotnetVersion_Returns()
    {
        string command = "dotnet version";

        Action action = () => _instance.ExecuteCommand(command);

        action.Should().Throw<ConsoleExecutionException>();
    }
}