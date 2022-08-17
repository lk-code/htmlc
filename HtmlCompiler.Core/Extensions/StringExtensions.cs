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
    }
}
