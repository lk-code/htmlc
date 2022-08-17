using FluentAssertions;
using HtmlCompiler.Core.Extensions;

namespace HtmlCompiler.Tests.Core.Extensions
{
    [TestClass]
    public class FileHandlingExtensionsTests
    {
        [TestMethod]
        public void FromSourceFilePath_OnlyOutputPath_ReturnsFullFilePath()
        {
            string sourceFilePath = "C:\\test\\source\\demo.html";
            string outputFilePath = "C:\\test\\output\\";

            string fullOutputFilePath = outputFilePath.FromSourceFilePath(sourceFilePath);

            fullOutputFilePath.Should().Be("C:\\test\\output\\demo.html");
        }

        [TestMethod]
        public void FromSourceFilePath_WithOutputFile_ReturnsFullFilePath()
        {
            string sourceFilePath = "C:\\test\\source\\demo.html";
            string outputFilePath = "C:\\test\\output\\test.html";

            string fullOutputFilePath = outputFilePath.FromSourceFilePath(sourceFilePath);

            fullOutputFilePath.Should().Be("C:\\test\\output\\test.html");
        }

        [TestMethod]
        public void FromSourceFilePath_OutputPathIsFileWithoutExtension_ReturnsFullFilePath()
        {
            string sourceFilePath = "C:\\test\\source\\demo.html";
            string outputFilePath = "C:\\test\\output";

            string fullOutputFilePath = outputFilePath.FromSourceFilePath(sourceFilePath);

            fullOutputFilePath.Should().Be("C:\\test\\output");
        }

        [TestMethod]
        public void FromSourceFilePath_SourceFileIsFileWithoutExtension_ReturnsFullFilePath()
        {
            string sourceFilePath = "C:\\test\\source\\base";
            string outputFilePath = "C:\\test\\output\\";

            string fullOutputFilePath = outputFilePath.FromSourceFilePath(sourceFilePath);

            fullOutputFilePath.Should().Be("C:\\test\\output\\base");
        }
    }
}
