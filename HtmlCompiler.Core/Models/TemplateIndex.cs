using System.Text.Json.Serialization;

namespace HtmlCompiler.Core.Models;

public class TemplateIndex
{
    [JsonPropertyName("templates")]
    public List<TemplateIndexEntry> Templates { get; set; } = new();
}