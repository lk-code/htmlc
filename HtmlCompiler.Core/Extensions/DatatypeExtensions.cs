using System;
namespace HtmlCompiler.Core.Extensions;

public static class DatatypeExtensions
{
    public static string Ensure(this string? val, string err)
    {
        if (string.IsNullOrEmpty(val))
        {
            throw new InvalidDataException(err);
        }

        return val.ToString();
    }
}