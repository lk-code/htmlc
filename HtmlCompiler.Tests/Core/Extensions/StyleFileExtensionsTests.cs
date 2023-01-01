using System;
using FluentAssertions;
using HtmlCompiler.Core.Extensions;

namespace HtmlCompiler.Tests.Core.Extensions;

[TestClass]
public class StyleFileExtensionsTests
{
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
}