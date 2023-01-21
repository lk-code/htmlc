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
}