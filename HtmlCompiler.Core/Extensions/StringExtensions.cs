using System.Text.RegularExpressions;
using System.Xml;

namespace HtmlCompiler.Core.Extensions
{
    public static class StringExtensions
    {
        public static string[] ToLines(this string content)
        {
            return content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public static string ToContent(this string[] lines)
        {
            return string.Join(Environment.NewLine, lines);
        }

        public static string AddMetaTag(this string html, string name, string content)
        {
            // Use regular expression to find the head tag
            string headTagPattern = @"<head.*?>";
            Regex headRegex = new Regex(headTagPattern, RegexOptions.IgnoreCase);

            // Create the new meta tag using the provided name and content parameters
            string newMetaTag = "<meta name='" + name + "' content='" + content + "'/>";

            // Check if the head tag already exists
            if (headRegex.IsMatch(html))
            {
                // Replace the closing head tag with the new meta tag and the closing head tag
                return headRegex.Replace(html, "$&" + newMetaTag);
            }
            else
            {
                // If the head tag does not exist, add it along with the new meta tag
                return "<head>" + newMetaTag + "</head>" + html;
            }
        }
    }
}
