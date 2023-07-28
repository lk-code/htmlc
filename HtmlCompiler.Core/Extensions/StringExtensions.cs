using HtmlAgilityPack;

namespace HtmlCompiler.Core.Extensions;

public static class StringExtensions
{
    public static string EnsureString(this string? val, string err)
    {
        if (string.IsNullOrEmpty(val))
        {
            throw new InvalidDataException(err);
        }

        return val.ToString();
    }
}