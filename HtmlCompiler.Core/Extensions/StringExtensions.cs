using HtmlAgilityPack;

namespace HtmlCompiler.Core.Extensions;

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
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head == null)
        {
            var htmlNode = doc.DocumentNode.SelectSingleNode("html");
            head = doc.CreateElement("head");
            htmlNode?.AppendChild(head);
        }

        var metaTag = doc.CreateElement("meta");
        metaTag.SetAttributeValue("name", name);
        metaTag.SetAttributeValue("content", content);
        head.AppendChild(metaTag);

        return doc.DocumentNode.OuterHtml;
    }

    public static string ReplaceCommentTags(this string html)
    {
        string pattern = "@Comment";
        var lines = html.Split(Environment.NewLine);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(pattern))
            {
                int commentIndex = lines[i].IndexOf(pattern);
                string comment = lines[i].Substring(commentIndex + pattern.Length);
                comment = comment.Trim().Replace("=", "");
                lines[i] = "<!-- " + comment + " -->";
            }
        }
        return string.Join(Environment.NewLine, lines);
    }
}