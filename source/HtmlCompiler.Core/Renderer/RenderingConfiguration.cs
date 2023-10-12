using System.Text.Json;

namespace HtmlCompiler.Core.Renderer;

public class RenderingConfiguration
{
    public string BaseDirectory { get; set; } = string.Empty;
    public string SourceDirectory { get; init; } = string.Empty;
    public string OutputDirectory { get; init; } = string.Empty;
    public string CssOutputFilePath { get; init; } = string.Empty;
    public string SourceFullFilePath { get; init; } = string.Empty;
    public JsonElement? GlobalVariables { get; init; } = null;
    public long CallLevel { get; init; } = 0;
}