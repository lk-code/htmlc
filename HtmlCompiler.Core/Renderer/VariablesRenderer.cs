using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer
{
    public class VariablesRenderer : RenderingBase
    {
        public const string VARIABLES_TAG = "@Var";

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
            Dictionary<string, object> jsonData = new Dictionary<string, object>();
            Regex regex = new Regex($"{VARIABLES_TAG}=(\\{{.*?\\}})", RegexOptions.Singleline);

            foreach (Match match in regex.Matches(content))
            {
                try
                {
                    string jsonObject = match.Groups[1].Value;
                    Dictionary<string, object> parsedObject = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject);
                    foreach (KeyValuePair<string, object> kvp in parsedObject)
                    {
                        if (jsonData.ContainsKey(kvp.Key))
                        {
                            // Handle duplicate keys here, if needed
                            Console.WriteLine($"Duplicate key found: {kvp.Key}");
                        }
                        else
                        {
                            jsonData[kvp.Key] = kvp.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any JSON parsing errors here
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }

            string jsonResult = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return jsonResult;
        }
    }
}
