using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;

namespace HtmlCompiler.Config;

public class ConfigurationManager : IConfigurationManager
{
    private readonly string _userConfigPath;
    private readonly IFileSystemService _fileSystemService;

    public ConfigurationManager(string userConfigPath,
        IFileSystemService fileSystemService)
    {
        _userConfigPath = userConfigPath ?? throw new ArgumentNullException(nameof(userConfigPath));
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    /// <inheritdoc />
    public async Task AddAsync(string key, string value)
    {
        ConfigModel userConfig = await this.EnsureAsync(await this.LoadAsync(this._userConfigPath));

        PropertyInfo? property = userConfig.GetType().GetProperties()
            .FirstOrDefault(prop =>
                prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == key &&
                prop.PropertyType == typeof(string[]));

        if (property is not null)
        {
            object? userConfigValue = property.GetValue(userConfig);

            if (userConfigValue is null)
            {
                throw new ArgumentException($"Property with name '{key}' not found or is not a string array with matching JSON property name.");
            }
            
            string[] arrayProperty = (string[])userConfigValue;
            string[] updatedArray = new string[arrayProperty.Length + 1];
            arrayProperty.CopyTo(updatedArray, 0);
            updatedArray[arrayProperty.Length] = value;
            property.SetValue(userConfig, updatedArray);
        }
        else
        {
            throw new ArgumentException($"Property with name '{key}' not found or is not a string array with matching JSON property name.");
        }

        // write user config file
        await this.WriteAsync(this._userConfigPath, userConfig);
    }

    /// <inheritdoc />
    public Task SetAsync(string key, string value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, string value)
    {
        ConfigModel userConfig = await this.EnsureAsync(await this.LoadAsync(this._userConfigPath));

        PropertyInfo? property = userConfig.GetType().GetProperties()
            .FirstOrDefault(prop =>
                prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == key &&
                prop.PropertyType == typeof(string[]));

        if (property is not null)
        {
            object? userConfigValue = property.GetValue(userConfig);

            if (userConfigValue is null)
            {
                throw new ArgumentException($"Property with name '{key}' not found or is not a string array with matching JSON property name.");
            }
            
            string[] arrayProperty = (string[])userConfigValue;
            string[] updatedArray = arrayProperty.Where(val => val != value).ToArray();
            property.SetValue(userConfig, updatedArray);
        }
        else
        {
            throw new ArgumentException($"Property with name '{key}' not found or is not a string array with matching JSON property name.");
        }
        
        // write user config file
        await this.WriteAsync(this._userConfigPath, userConfig);
    }

    private async Task WriteAsync(string userConfigPath, ConfigModel userConfig)
    {
        string userJsonConfig = JsonSerializer.Serialize(userConfig);
        await this._fileSystemService.FileWriteAllTextAsync(userConfigPath, userJsonConfig);
    }

    private async Task<ConfigModel?> LoadAsync(string userConfigPath)
    {
        string userJsonConfig = await this._fileSystemService.FileReadAllTextAsync(userConfigPath);
        ConfigModel? userConfig = JsonSerializer.Deserialize<ConfigModel>(userJsonConfig);

        return userConfig;
    }

    private async Task<ConfigModel> EnsureAsync(ConfigModel? userConfig)
    {
        await Task.CompletedTask;
        
        if (userConfig is null)
        {
            // no config
            userConfig = new ConfigModel();
        }

        return userConfig;
    }

    private string GetJsonPropertyName(PropertyInfo property)
    {
        JsonPropertyNameAttribute? attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();

        if (attribute is not null)
        {
            return attribute.Name;
        }

        return property.Name;
    }
}