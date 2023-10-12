namespace HtmlCompiler.Core.Extensions;

public static class FileHandlingExtensions
{
    /// <summary>
    /// returns a full output path with a filename
    /// </summary>
    /// <param name="outputFilePath"></param>
    /// <param name="sourceFilePath"></param>
    /// <returns></returns>
    public static string FromSourceFilePath(this string outputFilePath, string sourceFilePath)
    {
        string sourceFileName = Path.GetFileName(sourceFilePath);
        string outputDirectoryPath = Path.GetDirectoryName(outputFilePath)!;

        string fullOutputFileName = outputFilePath;
        if (string.IsNullOrEmpty(Path.GetFileName(outputFilePath)))
        {
            fullOutputFileName = Path.Combine(outputDirectoryPath, sourceFileName);
        }

        return fullOutputFileName;
    }
}
