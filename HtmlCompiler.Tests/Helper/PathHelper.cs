using System;
namespace HtmlCompiler.Tests.Helper;

public static class PathHelper
{
    public static string ToSystemPath(this string path)
    {
        return path.Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
    }
}