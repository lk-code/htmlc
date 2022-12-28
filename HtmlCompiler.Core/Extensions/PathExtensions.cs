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
}