using HtmlCompiler.Core.Exceptions;

namespace HtmlCompiler.Core.Extensions;

public static class StyleFileExtensions
{
    public static bool IsSupportedStyleFileOrThrow(this string fileExtension)
    {
        string ext = fileExtension.ToLowerInvariant().TrimStart('.');
        if (ext == "scss"
            || ext == "sass"
            || ext == "less")
        {
            return true;
        }

        throw new UnsupportedStyleTypeException("style type is not supported (only sass, scss and less is supported to compile)");
    }
}