using System.Text.Json;
using System.Text.RegularExpressions;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer
{
    public class VariablesRenderer : RenderingBase
    {
        public const string VARIABLES_TAG = "@Var";

        public override bool PreRenderPartialFiles { get; } = false;

        public VariablesRenderer(RenderingConfiguration configuration,
            IFileSystemService fileSystemService,
            IHtmlRenderer htmlRenderer)
            : base(configuration,
                fileSystemService,
                htmlRenderer)
        {
        }

        public override async Task<string> RenderAsync(string content)
        {
            JsonDocument json = LoadVariables(ref content);

            string jsonString = json.RootElement.GetRawText();

            content = await ReplaceVariables(content, json);

            return content;
        }

        private async Task<string> ReplaceVariables(string content, JsonDocument json)
        {
            string pattern = $@"{VARIABLES_TAG}\[(.*?)\];";
            Regex regex = new Regex(pattern);

            string result = regex.Replace(content, match =>
            {
                string[] keyPath = GetKeyPath(match.Value);

                string? value = FindJsonValue(json.RootElement, keyPath);
                return value ?? match.Value;
            });

            return result;
        }

        private static string[] GetKeyPath(string key)
        {
            string keyValue = key.Substring(VARIABLES_TAG.Length)
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
                    
                    element = element.EnumerateArray().ElementAtOrDefault((int) index);
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

        private static JsonDocument LoadVariables(ref string content)
        {
            Dictionary<string, object> jsonData = new Dictionary<string, object>();
            string[] lines = content.Split('\n');
            List<string> updatedLines = new List<string>();

            foreach (string line in lines)
            {
                if (line.Trim().StartsWith(VARIABLES_TAG))
                {
                    string jsonObject = line.Trim().Substring(VARIABLES_TAG.Length + 1).Trim();

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

            return new DataBuilder().LoadFrom(jsonResult).Build();
        }
    }
}