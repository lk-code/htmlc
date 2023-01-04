using System;
namespace HtmlCompiler;

public class ConfigModel
{
    public string[] BuildBlackList { get; set; } = Array.Empty<string>();

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