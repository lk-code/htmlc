using System;
using FluentAssertions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using Moq;

namespace HtmlCompiler.Tests.Core.Extensions;

[TestClass]
public class StyleFileExtensionsTests
{
    private Mock<IStyleCompiler> _styleCompiler = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._styleCompiler = new Mock<IStyleCompiler>();
    }

    [TestMethod]
    public void GetFullObjectNameBySassImportName_WithFullFileName_Returns()
    {
        string sassImport = "components/_buttons.scss";

        string result = sassImport.GetFullObjectNameBySassImportName("scss");

        result.Should().NotBeNull();
        result.Should().Be("components/_buttons.scss");
    }

    [TestMethod]
    public void GetFullObjectNameBySassImportName_WithSimpleFileName_Returns()
    {
        string sassImport = "components/buttons";

        string result = sassImport.GetFullObjectNameBySassImportName("scss");

        result.Should().NotBeNull();
        result.Should().Be("components/_buttons.scss");
    }

    [TestMethod]
    public void GetFullObjectNameBySassImportName_WithSingleSimpleFileName_Returns()
    {
        string sassImport = "components";

        string result = sassImport.GetFullObjectNameBySassImportName("scss");

        result.Should().NotBeNull();
        result.Should().Be("_components.scss");
    }

    [TestMethod]
    public void GetFullObjectNameBySassImportName_WithFullFilePath_Returns()
    {
        string sassImport = "../../plugins/template/main.scss";

        string result = sassImport.GetFullObjectNameBySassImportName("scss");

        result.Should().NotBeNull();
        result.Should().Be("../../plugins/template/main.scss");
    }

    [TestMethod]
    public void GetFullObjectNameBySassImportName_WithFullFilePathSimplified_Returns()
    {
        string sassImport = "../../plugins/template/header";

        string result = sassImport.GetFullObjectNameBySassImportName("scss");

        result.Should().NotBeNull();
        result.Should().Be("../../plugins/template/_header.scss");
    }

    [TestMethod]
    public void GetFullObjectNameBySassImportName_WithFullUnderscoredFilePath_Returns()
    {
        string sassImport = "../../plugins/template/_data.scss";

        string result = sassImport.GetFullObjectNameBySassImportName("scss");

        result.Should().NotBeNull();
        result.Should().Be("../../plugins/template/_data.scss");
    }

    [TestMethod]
    public async Task ReplaceSassImports_WithImportAndQuotationMark_Returns()
    {
        string testCss = "body {" + Environment.NewLine +
            "color: red;" + Environment.NewLine +
            "}" + Environment.NewLine +
            "@import \"_test.scss\";";
        string loadedCss = ".test {" + Environment.NewLine +
            "color: blue;" + Environment.NewLine +
            "}";
        string expectedCss = "body {" + Environment.NewLine +
            "color: red;" + Environment.NewLine +
            "}" + Environment.NewLine +
            ".test {" + Environment.NewLine +
            "color: blue;" + Environment.NewLine +
            "}";

        this._styleCompiler.Setup(x => x.GetStyleContent(It.IsAny<string>(), "_test.scss"))
            .ReturnsAsync(loadedCss);

        string result = await testCss.ReplaceSassImports(this._styleCompiler.Object, "", "", "");

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedCss);
    }

    [TestMethod]
    public async Task ReplaceSassImports_WithImportAndApostrophe_Returns()
    {
        string testCss = "body {" + Environment.NewLine +
            "color: red;" + Environment.NewLine +
            "}" + Environment.NewLine +
            "@import '_test.scss';";
        string loadedCss = ".test {" + Environment.NewLine +
            "color: blue;" + Environment.NewLine +
            "}";
        string expectedCss = "body {" + Environment.NewLine +
            "color: red;" + Environment.NewLine +
            "}" + Environment.NewLine +
            ".test {" + Environment.NewLine +
            "color: blue;" + Environment.NewLine +
            "}";

        this._styleCompiler.Setup(x => x.GetStyleContent(It.IsAny<string>(), "_test.scss"))
            .ReturnsAsync(loadedCss);

        string result = await testCss.ReplaceSassImports(this._styleCompiler.Object, "", "", "");

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedCss);
    }
    
    [TestMethod]
    public void IsSupportedStyleFile_ReturnsTrue_ForSCSSFile()
    {
        // Arrange
        string fileExtension = ".scss";

        // Act
        bool result = fileExtension.IsSupportedStyleFile();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsSupportedStyleFile_ReturnsTrue_ForSASSFile()
    {
        // Arrange
        string fileExtension = "sass";

        // Act
        bool result = fileExtension.IsSupportedStyleFile();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsSupportedStyleFile_ReturnsTrue_ForLESSFile()
    {
        // Arrange
        string fileExtension = "less";

        // Act
        bool result = fileExtension.IsSupportedStyleFile();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsSupportedStyleFile_ReturnsFalse_ForUnsupportedFile()
    {
        // Arrange
        string fileExtension = "txt";

        // Act
        bool result = fileExtension.IsSupportedStyleFile();

        // Assert
        result.Should().BeFalse();
    }
}