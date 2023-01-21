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
}