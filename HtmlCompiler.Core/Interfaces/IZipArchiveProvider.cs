namespace HtmlCompiler.Core.Interfaces;

public interface IZipArchiveProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="files"></param>
    /// <param name="rootDirectory"></param>
    /// <param name="outputFilePath"></param>
    /// <returns></returns>
    IReadOnlyCollection<string> CreateZipFile(IEnumerable<string> files, string rootDirectory, string outputFilePath);
}