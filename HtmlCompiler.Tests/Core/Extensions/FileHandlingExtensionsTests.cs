using FluentAssertions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Tests.Helper;

namespace HtmlCompiler.Tests.Core.Extensions;

[TestClass]
public class FileHandlingExtensionsTests
{
    [TestMethod]
    public void FromSourceFilePath_OnlyOutputPath_ReturnsFullFilePath()
    {
        string sourceFilePath = $"C:\\test\\source\\demo.html".ToSystemPath();
        string outputFilePath = "C:\\test\\output\\".ToSystemPath();

        string fullOutputFilePath = outputFilePath.FromSourceFilePath(sourceFilePath);

        fullOutputFilePath.Should().Be("C:\\test\\output\\demo.html".ToSystemPath());
    }

    [TestMethod]
    public void FromSourceFilePath_WithOutputFile_ReturnsFullFilePath()
    {
        string sourceFilePath = "C:\\test\\source\\demo.html".ToSystemPath();
        string outputFilePath = "C:\\test\\output\\test.html".ToSystemPath();

        string fullOutputFilePath = outputFilePath.FromSourceFilePath(sourceFilePath);

        fullOutputFilePath.Should().Be("C:\\test\\output\\test.html".ToSystemPath());
    }

    [TestMethod]
    public void FromSourceFilePath_OutputPathIsFileWithoutExtension_ReturnsFullFilePath()
    {
        string sourceFilePath = "C:\\test\\source\\demo.html".ToSystemPath();
        string outputFilePath = "C:\\test\\output".ToSystemPath();

        string fullOutputFilePath = outputFilePath.FromSourceFilePath(sourceFilePath);

        fullOutputFilePath.Should().Be("C:\\test\\output".ToSystemPath());
    }

    [TestMethod]
    public void FromSourceFilePath_SourceFileIsFileWithoutExtension_ReturnsFullFilePath()
    {
        string sourceFilePath = "C:\\test\\source\\base".ToSystemPath();
        string outputFilePath = "C:\\test\\output\\".ToSystemPath();

        string fullOutputFilePath = outputFilePath.FromSourceFilePath(sourceFilePath);

        fullOutputFilePath.Should().Be("C:\\test\\output\\base".ToSystemPath());
    }
}
