using Cocona;
using HtmlCompiler.Config;
using HtmlCompiler.Core.Extensions;

namespace HtmlCompiler.Commands;

public class ConfigCommand
{
    private readonly IConfigurationManager _configurationManager;

    public ConfigCommand(IConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
    }

    [Command("config")]
    public async Task Config([Argument(Description = "the config value to edit")] string key,
        [Argument(Description = "the action on array-based config entries (add, remove or replace)")]
        string? action,
        [Argument(Description = "the config value as json string")]
        string? value)
    {
        try
        {
            action = action.EnsureString($"action is needed for {key}!");

            switch (action.ToLowerInvariant())
            {
                case "add":
                {
                    await this._configurationManager.AddAsync(key, value.EnsureString($"value for {key} can not be empty!"));
                }
                    break;
                case "remove":
                {
                    await this._configurationManager.RemoveAsync(key, value.EnsureString($"value for {key} can not be empty!"));
                }
                    break;
                case "set":
                {
                    await this._configurationManager.SetAsync(key, value.EnsureString($"value for {key} can not be empty!"));
                }
                    break;
                default:
                {
                    throw new ArgumentException($"unknown action: {action}");
                }
                    break;
            }
        }
        catch (Exception err)
        {
            Console.WriteLine($"ERROR: {err.Message}");
        }
    }
}