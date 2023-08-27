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