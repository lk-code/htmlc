using System.Text.Json;
using System.Text.RegularExpressions;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core.Renderer;

public class VariablesRenderer : RenderingBase
{
    public const string VARIABLES_SET_TAG = "@Var=";
    public const string VARIABLES_TAG = "@Var";
    public const string VARIABLES_FILE_TAG = "@VarFile=";

    public override bool PreRenderPartialFiles { get; } = false;

    public VariablesRenderer(ILogger<VariablesRenderer> logger,
        RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(logger,
            configuration,
            fileSystemService,
            htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        (JsonElement json, string content) jsonInline = LoadVariables(content);
        content = jsonInline.content;
        IDataBuilder jsonInlineBuilder = new DataBuilder().LoadFrom(jsonInline.json);
        
        (JsonElement json, string content) jsonFile = await LoadVariablesFromFileAsync(content);
        content = jsonFile.content;
        IDataBuilder jsonFileBuilder = new DataBuilder().LoadFrom(jsonFile.json);
        
        IDataBuilder mergedBuilder = DataBuilder.Merge(jsonInlineBuilder, jsonFileBuilder);
        JsonElement mergedJson = mergedBuilder.Build().RootElement;

        content = await ReplaceVariables(content, mergedJson);

        return content;
    }

    private async Task<string> ReplaceVariables(string content, JsonElement json)
    {
        string pattern = $@"{VARIABLES_TAG}\[(.*?)\];";
        Regex regex = new Regex(pattern);

        string result = regex.Replace(content, match =>
        {
            string[] keyPath = GetKeyPath(match.Value);

            string? value = FindJsonValue(json, keyPath);
            return value ?? match.Value;
        });

        return result;
    }

    private static string[] GetKeyPath(string key)
    {
        string keyValue = key.Substring(VARIABLES_SET_TAG.Length)
            .TrimEnd(';')
            .TrimStart('[')
            .TrimStart('"')
            .TrimEnd(']')
            .TrimEnd('"');
        string[] keyPath = keyValue.Split(':')
            .Select(x => x
                .TrimStart('[')
                .TrimEnd(']'))
            .ToArray();
        return keyPath;
    }

    private string FindJsonValue(JsonElement element, string[] keyPath)
    {
        foreach (string key in keyPath)
        {
            if (element.ValueKind == JsonValueKind.Object
                && element.TryGetProperty(key, out var property))
            {
                element = property;
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                long index = long.Parse(key);

                element = element.EnumerateArray().ElementAtOrDefault((int)index);
            }
            else
            {
                return null;
            }
        }

        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                return element.GetRawText();
            default:
                return null;
        }
    }

    private static (JsonElement, string) LoadVariables(string content)
    {
        Dictionary<string, object> jsonData = new Dictionary<string, object>();
        string[] lines = content.Split('\n');
        List<string> updatedLines = new List<string>();

        foreach (string line in lines)
        {
            if (line.Trim().StartsWith(VARIABLES_SET_TAG))
            {
                string jsonObject = line.Trim().Substring(VARIABLES_SET_TAG.Length).Trim();

                try
                {
                    Dictionary<string, object> parsedObject =
                        JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject);
                    foreach (KeyValuePair<string, object> kvp in parsedObject)
                    {
                        jsonData[kvp.Key] = kvp.Value;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }
            else
            {
                updatedLines.Add(line);
            }
        }

        string jsonResult = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        content = string.Join('\n', updatedLines);
        JsonElement json = new DataBuilder().LoadFrom(jsonResult).Build().RootElement;
        
        return (json, content);
    }

    private async Task<(JsonElement, string)> LoadVariablesFromFileAsync(string content)
    {
        IDataBuilder json = new DataBuilder();

        string[] lines = content.Split('\n');
        List<string> updatedLines = new List<string>();

        foreach (string line in lines)
        {
            if (line.Trim().StartsWith(VARIABLES_FILE_TAG))
            {
                string variableFileName = line.Trim().Substring(VARIABLES_FILE_TAG.Length).Trim();
                string fullPath = Path.Combine(this._configuration.BaseDirectory, variableFileName);

                if (!this._fileSystemService.FileExists(fullPath))
                {
                    updatedLines.Add(line);
                    continue;
                }

                string variableFileContent = await this._fileSystemService.FileReadAllTextAsync(fullPath);
                JsonElement element = new DataBuilder().LoadFrom(variableFileContent)
                    .Build().RootElement;

                json = DataBuilder.Merge(json, new DataBuilder().LoadFrom(element));
            }
            else
            {
                updatedLines.Add(line);
            }
        }

        content = string.Join('\n', updatedLines);

        return (json.Build().RootElement, content);
    }
}