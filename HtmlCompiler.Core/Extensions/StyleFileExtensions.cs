using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Extensions;

public static class StyleFileExtensions
{
    public static bool IsSupportedStyleFile(this string fileExtension)
    {
        string ext = fileExtension.ToLowerInvariant();
        if (ext == ".scss"
            || ext == "sass"
            || ext == "less")
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// components/_butttons.scss       => /components/_butttons.scss
    /// components/butttons             => /components/_butttons.scss
    /// </summary>
    /// <param name="sassImportName"></param>
    /// <param name="requestedFileExtension"></param>
    /// <returns></returns>
    public static string GetFullObjectNameBySassImportName(this string sassImportName, string requestedFileExtension)
    {
        if (!requestedFileExtension.StartsWith("."))
        {
            requestedFileExtension = $".{requestedFileExtension}";
        }

        string? directory = Path.GetDirectoryName(sassImportName);
        if (string.IsNullOrEmpty(directory))
        {
            directory = string.Empty;
        }
        string fileName = Path.GetFileNameWithoutExtension(sassImportName);
        string fileExtension = Path.GetExtension(sassImportName);

        if (!string.IsNullOrEmpty(fileExtension))
        {
            return sassImportName;
        }

        fileExtension = requestedFileExtension;

        if (!fileName.StartsWith("_"))
        {
            fileName = "_" + fileName;
        }

        string modifiedFilePath = Path.Combine(directory, $"{fileName}{fileExtension}");

        return modifiedFilePath;
    }

    public static async Task<string> ReplaceSassImports(this string input,
        IStyleCompiler styleCompiler,
        string sourceDirectoryPath,
        string currentSubDirectory,
        string fileExtension)
    {
        Regex importRegex = new Regex(@"@import\s+['""]([^'""\n\r]+)['""]\s*;", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        foreach (Match match in importRegex.Matches(input))
        {
            string requestedSassFile = match.Groups[1].Value;
            requestedSassFile = requestedSassFile.GetFullObjectNameBySassImportName(fileExtension);

            requestedSassFile = Path.Combine(sourceDirectoryPath, currentSubDirectory, requestedSassFile);

            string replacement = await styleCompiler.GetStyleContent(sourceDirectoryPath, requestedSassFile);

            input = input.Replace(match.Value, replacement);
        }

        return input;
    }
}