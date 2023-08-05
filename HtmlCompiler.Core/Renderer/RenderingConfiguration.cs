namespace HtmlCompiler.Core.Renderer;

public class RenderingConfiguration
{
    public string BaseDirectory { get; set; } = string.Empty;
    public string SourceDirectory { get; init; } = string.Empty;
    public string OutputDirectory { get; init; } = string.Empty;
    public string CssOutputFilePath { get; init; } = string.Empty;
    public string SourceFullFilePath { get; init; } = string.Empty;
}