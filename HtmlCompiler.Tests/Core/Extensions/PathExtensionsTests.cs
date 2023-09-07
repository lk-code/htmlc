using FluentAssertions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Tests.Helper;

namespace HtmlCompiler.Tests.Core.Extensions;

[TestClass]
public class PathExtensionsTests
{
    [TestMethod]
    public void GetRelativePath_WithOneLevelEntryFile_ReturnsRelativeFilePath()
    {
        string baseDirectory = "/Users/larskramer/Desktop/htmlc-test/dist/".ToSystemPath();
        string entryFilePath = "/Users/larskramer/Desktop/htmlc-test/dist/_additional.html".ToSystemPath();
        string targetFilePath = "/Users/larskramer/Desktop/htmlc-test/dist/styles/main.css".ToSystemPath();

        string actualRelativePath = entryFilePath.GetRelativePath(baseDirectory, targetFilePath);

        actualRelativePath.Should().NotBeNullOrEmpty();
        actualRelativePath.Should().Be("styles/main.css".ToSystemPath());
    }

    [TestMethod]
    public void GetRelativePath_WithMultiLevelEntryFile_ReturnsRelativeFilePath()
    {
        string baseDirectory = "/Users/larskramer/Desktop/htmlc-test/dist/".ToSystemPath();
        string entryFilePath = "/Users/larskramer/Desktop/htmlc-test/dist/pages/controls/buttons.html".ToSystemPath();
        string targetFilePath = "/Users/larskramer/Desktop/htmlc-test/dist/styles/main.css".ToSystemPath();

        string actualRelativePath = entryFilePath.GetRelativePath(baseDirectory, targetFilePath);

        actualRelativePath.Should().NotBeNullOrEmpty();
        actualRelativePath.Should().Be("../../styles/main.css".ToSystemPath());
    }
}