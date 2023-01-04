using System;
using System.Text.Json;
using Cocona;
using HtmlCompiler.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Commands;

public class ConfigCommand
{
    private readonly IConfiguration _configuration;

    public ConfigCommand(IConfiguration configuration)
    {
        this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [Command("config")]
    public async Task Config([Argument(Description = "the config value to edit")] string key,
        [Argument(Description = "the config value as json string")] string? value = null)
    {
        // read user config file
        string userJsonConfig = await File.ReadAllTextAsync(Globals.USER_CONFIG);
        ConfigModel? userConfig = JsonSerializer.Deserialize<ConfigModel>(userJsonConfig);
        if (userConfig == null)
        {
            // no config
            userConfig = new ConfigModel();
        }

        // edit the content
        try
        {
            userConfig = this.EditOnConfig(userConfig, key, value);
        }
        catch (Exception err)
        {
            Console.WriteLine($"error: {err.Message}");
            return;
        }

        if (userConfig == null)
        {
            return;
        }

        // write user config file
        await File.WriteAllTextAsync(Globals.USER_CONFIG, JsonSerializer.Serialize(userConfig));
    }

    private ConfigModel? EditOnConfig(ConfigModel userConfig, string key, string? value)
    {
        switch (key.ToLowerInvariant())
        {
            case "tests":
                {
                    userConfig.Tests = value.Ensure("value for tests cant be empty!");
                }
                break;
            default:
                {
                    Console.WriteLine($"unknown config key: {key}");

                    return null;
                }
        }

        return userConfig;
    }
}