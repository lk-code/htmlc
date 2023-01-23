using FluentAssertions;
using HtmlCompiler.Core;

namespace HtmlCompiler.Tests;

[TestClass]
public class HtmlRendererTests
{
    private HtmlRenderer _instance = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._instance = new HtmlRenderer();
    }

    [TestMethod]
    public void RenderHtmlEscapeBlocks_WithSimpleHtml_Returns()
    {
        string html = "<section>" + Environment.NewLine +
            "@StartHtmlSpecialChars" + Environment.NewLine +
            "<h1 class=\"title\">" + Environment.NewLine +
            "<a href=\"https://a.link.testcase\">a link</a>" + Environment.NewLine +
            "</h1>" + Environment.NewLine +
            "@EndHtmlSpecialChars" + Environment.NewLine +
            "</section>";
        string expectedHtml = "<section>" + Environment.NewLine +
            "<br>" + Environment.NewLine +
            "&#60;h1 class=&#34;title&#34;&#62;<br>" + Environment.NewLine +
            "&#60;a href=&#34;https://a.link.testcase&#34;&#62;a link&#60;/a&#62;<br>" + Environment.NewLine +
            "&#60;/h1&#62;<br>" + Environment.NewLine +
            "" + Environment.NewLine +
            "</section>";

        string result = this._instance.RenderHtmlEscapeBlocks(html);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }

    [TestMethod]
    public void RenderHtmlEscapeBlocks_WithMultipleTags_Returns()
    {
        string html = "<section>" + Environment.NewLine +
            "@StartHtmlSpecialChars" + Environment.NewLine +
            "<h1 class=\"title\">" + Environment.NewLine +
            "<a href=\"https://a.link.testcase\">a link</a>" + Environment.NewLine +
            "</h1>" + Environment.NewLine +
            "@EndHtmlSpecialChars" + Environment.NewLine +
            "</section>" + Environment.NewLine +
            "<section>" + Environment.NewLine +
            "@StartHtmlSpecialChars" + Environment.NewLine +
            "<h1 class=\"title\">" + Environment.NewLine +
            "<a href=\"https://a.link.testcase\">a link</a>" + Environment.NewLine +
            "</h1>" + Environment.NewLine +
            "@EndHtmlSpecialChars" + Environment.NewLine +
            "</section>";
        string expectedHtml = "<section>" + Environment.NewLine +
            "<br>" + Environment.NewLine +
            "&#60;h1 class=&#34;title&#34;&#62;<br>" + Environment.NewLine +
            "&#60;a href=&#34;https://a.link.testcase&#34;&#62;a link&#60;/a&#62;<br>" + Environment.NewLine +
            "&#60;/h1&#62;<br>" + Environment.NewLine +
            "" + Environment.NewLine +
            "</section>" + Environment.NewLine +
            "<section>" + Environment.NewLine +
            "<br>" + Environment.NewLine +
            "&#60;h1 class=&#34;title&#34;&#62;<br>" + Environment.NewLine +
            "&#60;a href=&#34;https://a.link.testcase&#34;&#62;a link&#60;/a&#62;<br>" + Environment.NewLine +
            "&#60;/h1&#62;<br>" + Environment.NewLine +
            "" + Environment.NewLine +
            "</section>";

        string result = this._instance.RenderHtmlEscapeBlocks(html);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }
}