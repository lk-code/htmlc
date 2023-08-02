﻿using System.Text.Json;
using Cocona;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Commands;

public class ConfigCommand
{
    private readonly IConfiguration _configuration;
    private readonly IFileSystemService _fileSystemService;

    public ConfigCommand(IConfiguration configuration,
        IFileSystemService fileSystemService)
    {
        this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    [Command("config")]
    public async Task Config([Argument(Description = "the config value to edit")] string key,
        [Argument(Description = "the action on array-based config entries (add, remove or replace)")] string? action,
        [Argument(Description = "the config value as json string")] string? value)
    {
        string userConfigPath = this._configuration.GetSection("Core:UserConfigPath").Get<string>();
        // read user config file
        string userJsonConfig = await this._fileSystemService.FileReadAllTextAsync(userConfigPath);
        // string userJsonConfig = await this._fileSystemService.FileReadAllTextAsync(Globals.USER_CONFIG);
        ConfigModel? userConfig = JsonSerializer.Deserialize<ConfigModel>(userJsonConfig);
        if (userConfig == null)
        {
            // no config
            userConfig = new ConfigModel();
        }

        // edit the content
        try
        {
            userConfig = this.EditOnConfig(userConfig, key, action, value);
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
        userJsonConfig = JsonSerializer.Serialize(userConfig);
        await this._fileSystemService.FileWriteAllTextAsync(userConfigPath, userJsonConfig);
        // await this._fileSystemService.FileWriteAllTextAsync(Globals.USER_CONFIG, userJsonConfig);
    }

    private ConfigModel? EditOnConfig(ConfigModel userConfig, string key, string? action, string? value)
    {
        string comparingKey = key.ToLowerInvariant();
        switch (comparingKey)
        {
            //case "bool-exmaple":
            //    {
            //        userConfig.Bool = true;
            //    }
            //    break;
            //case "string-exmaple":
            //    {
            //        userConfig = value.Ensure($"value for {comparingKey} can not be empty!");
            //    }
            //    break;
            case "build-blacklist":
                {
                    EditBuildBlacklist(userConfig, ref action, ref value, comparingKey);
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

    private static void EditBuildBlacklist(ConfigModel userConfig, ref string? action, ref string? value, string comparingKey)
    {
        List<string> blacklist = userConfig.BuildBlackList.ToList();
        action = action.EnsureString($"action is needed for {comparingKey}!");
        value = value.EnsureString($"value for {comparingKey} can not be empty!");

        if (action.ToLowerInvariant() == "add")
        {
            // add
            blacklist.Add(value!.ToLowerInvariant());
        }
        else if (action.ToLowerInvariant() == "remove")
        {
            // remove
            blacklist.Remove(value!.ToLowerInvariant());
        }
        else if (action.ToLowerInvariant() == "replace")
        {
            // overwrite
            blacklist = JsonSerializer.Deserialize<string[]>(value!)!.ToList();
        }

        userConfig.BuildBlackList = blacklist.ToArray();
    }
}