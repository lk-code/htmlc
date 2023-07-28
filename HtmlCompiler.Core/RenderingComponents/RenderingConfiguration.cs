using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.RenderingComponents;

public class RenderingConfiguration
{
    public string BaseDirectory { get; set; }
    public string SourceDirectory { get; init; }
    public string OutputDirectory { get; init; }
    public string CssOutputFilePath { get; init; }
    public string SourceFullFilePath { get; init; }
}