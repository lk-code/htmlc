using System;
using FluentAssertions;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using Moq;

namespace HtmlCompiler.Tests.Core.Extensions;

[TestClass]
public class StyleFileExtensionsTests
{
    private Mock<IStyleManager> _styleCompiler = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._styleCompiler = new Mock<IStyleManager>();
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
    [DataRow("sass")]
    [DataRow("scss")]
    [DataRow("less")]
    public void IsSupportedStyleFileOrThrow_WithExplicitSupported_Returns(string fileExtension)
    {
        // Act
        bool result = fileExtension.IsSupportedStyleFileOrThrow();

        // Assert
        result.Should().BeTrue();
    }
    
    [TestMethod]
    [DataRow("test.sass")]
    [DataRow("test.scss")]
    [DataRow("test.less")]
    public void IsSupportedStyleFileOrThrow_WithSupported_Returns(string fileName)
    {
        // Arrange
        // fileExtension is some like ".sass", ".scss" or ".less" (with leading dot)
        string fileExtension = Path.GetExtension(fileName);

        // Act
        bool result = fileExtension.IsSupportedStyleFileOrThrow();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    [DataRow("test.txt")]
    [DataRow("test.docx")]
    [DataRow("test.exe")]
    [DataRow("test.css")]
    public void IsSupportedStyleFileOrThrow_WithUnsupported_Throws(string fileName)
    {
        // Arrange
        string fileExtension = Path.GetExtension(fileName);

        // Act
        Action act = () => fileExtension.IsSupportedStyleFileOrThrow();
        
        // Assert
        act.Should().Throw<UnsupportedStyleTypeException>();
    }
}