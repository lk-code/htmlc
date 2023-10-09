namespace HtmlCompiler.Core.Interfaces;

public interface ITemplatePackagingService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="outputPath"></param>
    /// <returns></returns>
    Task CreateAsync(string sourcePath, string? outputPath = null);
}