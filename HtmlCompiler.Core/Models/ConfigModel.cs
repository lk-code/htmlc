using System.Text.Json.Serialization;

namespace HtmlCompiler.Core.Models;

public class ConfigModel
{
    [JsonPropertyName("project-global-file")]
    public string ProjectGlobalFile { get; set; } = string.Empty;

    [JsonPropertyName("build-blacklist")]
    public string[] BuildBlackList { get; set; } = Array.Empty<string>();

    [JsonPropertyName("template-repositories")]
    public string[] TemplateRepositories { get; set; } = Array.Empty<string>();
    
    [JsonPropertyName("style-commands")]
    public Dictionary<string, string> StyleCommands { get; set; } = new();

    public static ConfigModel GetBasicConfig()
    {
        ConfigModel configModel = new ConfigModel();

        configModel.BuildBlackList = new List<string>
        {
            ".scss",
            ".sass",
            ".less"
        }.ToArray();

        return configModel;
    }
}