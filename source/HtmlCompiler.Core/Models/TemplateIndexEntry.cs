using System.Text.Json.Serialization;

namespace HtmlCompiler.Core.Models;

public class TemplateIndexEntry
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("file")]
    public string File { get; set; } = string.Empty;
}