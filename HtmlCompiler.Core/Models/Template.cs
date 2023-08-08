namespace HtmlCompiler.Core.Models;

public record Template
{
    public string Name { get; set; }
    public string FileName { get; set; }
    public string Url { get; set; }
}