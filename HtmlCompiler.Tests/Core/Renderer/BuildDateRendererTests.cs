using System.Globalization;
using FluentAssertions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Renderer;
using NSubstitute;

namespace HtmlCompiler.Tests.Core.Renderer;

[TestClass]
public class BuildDateRendererTests
{
    private BuildDateRenderer _instance = null!;
    private IHtmlRenderer _htmlRenderer = null!;

    [TestInitialize]
    public void SetUp()
    {
        this._htmlRenderer = Substitute.For<IHtmlRenderer>();
        
        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = @"C:\",
            SourceDirectory = @"C:\",
            OutputDirectory = @"C:\",
            CssOutputFilePath = @"C:\"
        };
        
        this._instance = new BuildDateRenderer(configuration,
            this._htmlRenderer);
        
        this._instance.DateTimeProvider.Now().Returns(new DateTime(2023, 4, 7, 16, 37, 41, 25));
    }

    [TestMethod]
    [DataRow("Copyright @BuildDate(\"yyyy\")", "Copyright 2023", "en-US")]
    [DataRow("current: <span>@BuildDate(\"dd.MM.yyyy\")</span>", "current: <span>07.04.2023</span>", "en-US")]
    [DataRow("@BuildDate", "4/7/2023 4:37:41PM", "en-US")]
    [DataRow("@BuildDate", "07.04.2023 16:37:41", "de-DE")]
    [DataRow("<pre>@BuildDate</pre>", "<pre>4/7/2023 4:37:41PM</pre>", "en-US")]
    public async Task RenderBuildDate_WithValidHtml_Returns(string html, string expectedHtml, string culture)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

        string result = await this._instance.RenderAsync(html);

        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedHtml);
    }
}