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
    public void AddMetaTag_WithDefaultHtml_Returns()
    {
        string sourceHtml = "<html><head><title>hello world</title></head><body><h1>hello world</h1></body></html>";

        string html = sourceHtml.AddMetaTag("generator", "htmlc test");

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").GetAttributeValue("content", "").Should().Be("htmlc test");
    }

    [TestMethod]
    public void AddMetaTag_WithoutHeadTag_Returns()
    {
        string sourceHtml = "<html><body><h1>hello world</h1></body></html>";

        string html = sourceHtml.AddMetaTag("generator", "htmlc test");

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").GetAttributeValue("content", "").Should().Be("htmlc test");
    }

    [TestMethod]
    public void AddMetaTag_BugWithHeaderInContent_Returns()
    {
        string sourceHtml = "<html><head><title>hello world</title></head><body><header><h1>hello world</h1></header/><p>hello world</p></body></html>";

        string html = sourceHtml.AddMetaTag("generator", "htmlc test");

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").Should().NotBeNull();
        htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='generator']").GetAttributeValue("content", "").Should().Be("htmlc test");
    }

    [TestMethod]
    public void ReplaceCommentTags_BugWithHeaderInContent_Returns()
    {
        string sourceHtml = "<html>" + Environment.NewLine +
            "<body>" + Environment.NewLine +
            "@Comment=START body" + Environment.NewLine +
            "<header>" + Environment.NewLine +
            "<h1>hello world</h1>" + Environment.NewLine +
            "</header>" + Environment.NewLine +
            "@Comment=END body" + Environment.NewLine +
            "</body>" + Environment.NewLine +
            "</html>";
        string expectedHtml = "<html>" + Environment.NewLine +
            "<body>" + Environment.NewLine +
            "<!-- START body -->" + Environment.NewLine +
            "<header>" + Environment.NewLine +
            "<h1>hello world</h1>" + Environment.NewLine +
            "</header>" + Environment.NewLine +
            "<!-- END body -->" + Environment.NewLine +
            "</body>" + Environment.NewLine +
            "</html>";

        string html = sourceHtml.ReplaceCommentTags();

        html.Should().NotBeNullOrEmpty();
        html.Should().Be(expectedHtml);
    }
    
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