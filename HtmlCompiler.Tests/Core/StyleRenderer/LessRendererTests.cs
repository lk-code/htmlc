using ExCSS;
using FluentAssertions;
using HtmlCompiler.Core.Models;
using HtmlCompiler.Core.StyleRenderer;

namespace HtmlCompiler.Tests.Core.StyleRenderer;

[TestClass]
public class LessRendererTests
{
    private LessRenderer _instance = null!;
    
    [TestInitialize]
    public void SetUp()
    {
        this._instance = new LessRenderer();
    }

    [TestMethod]
    public async Task Compile_WithSimple_Return()
    {
        string styleContent = "body { color: red; }";
        StyleRenderingResult result = await this._instance.Compile(styleContent);

        result.Should().NotBeNull();

        string mapResult = result.MapResult.Replace(Environment.NewLine, "").Trim();
        mapResult.Should().NotBeEmpty();
        
        string styleResult = result.StyleResult.Replace(Environment.NewLine, "").Trim();
        styleResult.Should().NotBeEmpty();
        
        StylesheetParser parser = new StylesheetParser();
        Stylesheet stylesheet = await parser.ParseAsync(styleResult);

        stylesheet.StyleRules.Count().Should().Be(1);
        
        var bodyRule = stylesheet.StyleRules.First() as StyleRule;
        bodyRule.SelectorText.Should().Be("body");
        bodyRule.Style.Color.Should().Be("rgb(255, 0, 0)");
    }
}