using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public class BuildDateRenderer : RenderingBase
{
    public const string BUILDDATE_TAG = "@BuildDate";

    public BuildDateRenderer(RenderingConfiguration configuration,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        DateTime now = this._htmlRenderer.DateTimeProvider.Now();
        
        string pattern = $@"{BUILDDATE_TAG}(\(""([^""]+)""\)?)?";
    
        string result = Regex.Replace(content, pattern, match =>
        {
            string format = match.Groups.Count > 2 ? match.Groups[2].Value : "G";
            return now.ToString(format);
        });

        return result;
    }
}