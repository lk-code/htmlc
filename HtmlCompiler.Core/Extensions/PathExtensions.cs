using System;
using System.Runtime.CompilerServices;

namespace HtmlCompiler.Core.Extensions;

public static class PathExtensions
{
    public static void EnsurePath(this string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static List<string> GetAllFiles(this string path)
    {
        var fileList = new List<string>();
        var directories = Directory.GetDirectories(path);
        fileList.AddRange(Directory.GetFiles(path));

        foreach (var directory in directories)
        {
            fileList.AddRange(directory.GetAllFiles());
        }

        return fileList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entryFilePath">the absolute path to the entry file (like: /path/to/project/subfolder/entry.html</param>
    /// <param name="baseDirectory">the absolute path to the base directory. like the root-directory of a webserver. (like: /path/to/project/)</param>
    /// <param name="targetFilePath">the absolute path to the target file (lile: /path/to/project/styles/main.css)</param>
    /// <returns>returns the relative path for <paramref name="targetFilePath"/> based on  the <paramref name="entryFilePath"/>. like ../styles/main.css</returns>
    public static string GetRelativePath(this string entryFilePath, string baseDirectory, string targetFilePath)
    {
        string relativeEntryPath = Path.GetRelativePath(baseDirectory, entryFilePath);
        relativeEntryPath = Path.GetDirectoryName(relativeEntryPath)!;
        string relativeTargetPath = Path.GetRelativePath(baseDirectory, targetFilePath);

        if (string.IsNullOrEmpty(relativeEntryPath))
        {
            return relativeTargetPath;
        }

        string relativeOutputPath = Path.GetRelativePath(relativeEntryPath, relativeTargetPath);

        return relativeOutputPath;
    }
}