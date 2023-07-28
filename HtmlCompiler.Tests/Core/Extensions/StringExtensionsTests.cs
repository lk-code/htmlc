using System;
using System.Xml.Linq;
using FluentAssertions;
using HtmlAgilityPack;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Tests.Helper;

namespace HtmlCompiler.Tests.Core.Extensions;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void EnsureString_Returns_InputString_For_NonNullInput()
    {
        // Arrange
        string inputString = "test string";
        string errorMessage = "Input string cannot be null or empty";

        // Act
        string result = inputString.EnsureString(errorMessage);

        // Assert
        result.Should().Be(inputString);
    }

    [TestMethod]
    public void EnsureString_Throws_InvalidDataException_For_NullInput()
    {
        // Arrange
        string? inputString = null;
        string errorMessage = "Input string cannot be null or empty";

        // Act
        Action action = () => inputString.EnsureString(errorMessage);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage(errorMessage);
    }

    [TestMethod]
    public void EnsureString_Throws_InvalidDataException_For_EmptyInput()
    {
        // Arrange
        string inputString = "";
        string errorMessage = "Input string cannot be null or empty";

        // Act
        Action action = () => inputString.EnsureString(errorMessage);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage(errorMessage);
    }
}