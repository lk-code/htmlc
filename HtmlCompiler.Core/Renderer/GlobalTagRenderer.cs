using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public class GlobalTagRenderer : RenderingBase
{
    public GlobalTagRenderer(RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            fileSystemService,
            htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        await Task.CompletedTask;
        
        string pattern = @"@Global:([a-zA-Z0-9:]+(?![a-zA-Z0-9:]))";
        Regex regex = new Regex(pattern);

        MatchCollection matches = regex.Matches(content);

        foreach (Match match in matches)
        {
            try
            {
                string globalKey = match.Groups[1].Value;
                string[] keyParts = globalKey.Split(':');

                JsonElement? currentElement = _configuration.GlobalVariables;

                foreach (string keyPart in keyParts)
                {
                    if (currentElement?.TryGetProperty(keyPart, out var nextElement) == true)
                    {
                        currentElement = nextElement;
                    }
                    else
                    {
                        currentElement = null;
                        break;
                    }
                }

                if (currentElement != null)
                {
                    string? globalValue = currentElement.ToString() ?? string.Empty;

                    StringBuilder extractedValues = new StringBuilder();
                    extractedValues.Append(globalValue);
                    extractedValues.Append(" ");

                    string extractedString = extractedValues.ToString().TrimEnd();
                    if (!string.IsNullOrEmpty(extractedString))
                    {
                        content = content.Replace(match.Value, extractedString);
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions as needed
            }
        }

        return content;
    }
}