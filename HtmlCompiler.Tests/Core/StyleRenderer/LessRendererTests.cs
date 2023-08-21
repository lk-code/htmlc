using System.Text;
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

    [TestMethod]
    public async Task GetImports_WithMultipleInline_Return()
    {
        string styleContent = "@import 'foundation/code', 'foundation/lists '; body { color: red; }";
        
        IEnumerable<string> importResults = await this._instance.GetImports(styleContent);
        
        importResults.Should().NotBeNull();
        importResults.Count().Should().Be(6);
        importResults.Should().Contain("foundation/code/");
        importResults.Should().Contain("foundation/code.less");
        importResults.Should().Contain("foundation/_code.less");
        importResults.Should().Contain("foundation/lists/");
        importResults.Should().Contain("foundation/lists.less");
        importResults.Should().Contain("foundation/_lists.less");
    }

    [TestMethod]
    public async Task GetImports_WithMultipleFormatted_Return()
    {
        string styleContent = new StringBuilder()
            .Append("@import \"scss/theme\";")
            .Append("@import \"scss/fonts\";")
            .Append("@import \"scss/theme\";")
            .Append("")
            .Append("body")
            .Append("{")
            .Append("color: red;")
            .Append("}")
            .ToString().Trim();
        
        IEnumerable<string> importResults = await this._instance.GetImports(styleContent);
        
        importResults.Should().NotBeNull();
        importResults.Count().Should().Be(6);
        importResults.Should().Contain("scss/fonts/");
        importResults.Should().Contain("scss/fonts.less");
        importResults.Should().Contain("scss/_fonts.less");
        importResults.Should().Contain("scss/theme/");
        importResults.Should().Contain("scss/theme.less");
        importResults.Should().Contain("scss/_theme.less");
    }
}