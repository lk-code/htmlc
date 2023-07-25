namespace HtmlCompiler.Core.Extensions;

public static class PathExtensions
{
    /// <summary>
    /// Gets the relative path from the entry file's directory to the target file's directory based on a specified base directory.
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

    /// <summary>
    /// Returns the base directory of the specified file path.
    /// </summary>
    /// <param name="sourceFile">The path to the file for which the base directory should be determined.</param>
    /// <returns>The base directory of the specified file.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the base directory is null or empty.</exception>
    public static string GetBaseDirectory(this string sourceFile)
    {
        string? baseDirectory = Path.GetDirectoryName(sourceFile);

        if (string.IsNullOrEmpty(baseDirectory))
        {
            throw new FileNotFoundException($"\"{nameof(baseDirectory)}\" cannot be NULL or empty.", nameof(baseDirectory));
        }

        return baseDirectory;
    }
}