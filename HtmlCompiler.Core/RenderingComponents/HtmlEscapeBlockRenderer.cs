using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.RenderingComponents;

public class HtmlEscapeBlockRenderer : RenderingBase
{
    public const string START_TAG = "@StartHtmlSpecialChars";
    public const string END_TAG = "@EndHtmlSpecialChars";

    public override async Task<string> RenderAsync(string content)
    {
        int startIndex = content.IndexOf(START_TAG);

        while (startIndex != -1)
        {
            int endIndex = content.IndexOf(END_TAG, startIndex);
            if (endIndex == -1)
            {
                endIndex = content.Length;
            }

            string textToEscape = content.Substring(startIndex + START_TAG.Length, endIndex - startIndex - START_TAG.Length);
            string escapedText = Regex.Replace(textToEscape, "[<>&\"']", m =>
            {
                switch (m.Value)
                {
                    case "<": return "&#60;";
                    case ">": return "&#62;";
                    case "&": return "&#38;";
                    case "\"": return "&#34;";
                    case "'": return "&#39;";
                }

                return m.Value;
            }, RegexOptions.None, TimeSpan.FromMilliseconds(100));
            escapedText = escapedText.Replace("\n", "<br>\n");
            content = content.Remove(startIndex, endIndex - startIndex + END_TAG.Length).Insert(startIndex, escapedText);

            startIndex = content.IndexOf(START_TAG, startIndex + escapedText.Length);
        }

        return content;
    }

    public HtmlEscapeBlockRenderer(RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            fileSystemService,
            htmlRenderer)
    {
    }
}