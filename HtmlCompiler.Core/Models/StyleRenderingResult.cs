namespace HtmlCompiler.Core.Models;

public class StyleRenderingResult
{
    public string StyleResult { get; } = string.Empty;
    public string MapResult { get; } = string.Empty;

    public StyleRenderingResult(string style, string map)
    {
        this.StyleResult = style;
        this.MapResult = map;
    }
}