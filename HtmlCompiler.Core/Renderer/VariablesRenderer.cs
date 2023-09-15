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

            content = await ReplaceVariables(content, json);

            return content;
        }

        private async Task<string> ReplaceVariables(string content, JsonDocument json)
        {
            string pattern = @"@Var\[(.*?)\]";
            Regex regex = new Regex(pattern);

            string result = regex.Replace(content, match =>
            {
                string keyWithBrackets = match.Groups[1].Value;
                string key = keyWithBrackets.Trim('[', ']', '"', '\''); // Entfernen Sie die eckigen Klammern
                string? value = FindJsonValue(json.RootElement, key);
                return value ?? match.Value;
            });

            return result;
        }

        private string FindJsonValue(JsonElement element, string keyPath)
        {
            string[] keys = keyPath.Split(':');
            foreach (string key in keys)
            {
                if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(key, out var property))
                {
                    element = property;
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
                        Dictionary<string, object> parsedObject = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject);
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
                    updatedLines.Add(line); // Behalte Zeilen ohne Variablen bei
                }
            }

            string jsonResult = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            content = string.Join('\n', updatedLines);

            return new DataBuilder().LoadFrom(jsonResult).Build();
        }
    }
}