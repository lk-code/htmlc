using System.Text.RegularExpressions;

namespace HtmlCompiler.Core.Extensions;

public static class TagExtensions
{
    /// <summary>
    /// Extracts and returns the path of the layout file referenced in the provided content.
    /// </summary>
    /// <param name="content">The content that may contain a reference to a layout file using the "@Layout" keyword.</param>
    /// <returns>
    /// The path of the layout file if a reference is found in the content; otherwise, returns null.
    /// </returns>
    /// <remarks>
    /// This method searches for the first occurrence of the "@Layout" keyword in the content, which is
    /// typically used to reference the layout file in certain templating systems. The layout file path
    /// should be placed after the "@Layout" keyword on the same line.
    /// The method extracts the layout file path from the content and returns it as a string.
    /// If no reference to a layout file is found, or if the layout path is not provided correctly, the method returns null.
    /// </remarks>
    public static string? GetLayoutFilePath(this string content)
    {
        Regex layoutRegex = new Regex("@Layout", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        Match layoutMatch = layoutRegex.Match(content);
        if (!layoutMatch.Success)
        {
            return null;
        }

        int lineBreakIndex = content.IndexOf(Environment.NewLine, layoutMatch.Index);
        if (lineBreakIndex < 0)
        {
            return null;
        }

        string layoutPath = content.Substring(layoutMatch.Index + 8, lineBreakIndex - layoutMatch.Index - 8).Trim();

        return layoutPath;
    }
}