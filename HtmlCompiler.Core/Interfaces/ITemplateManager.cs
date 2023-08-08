using HtmlCompiler.Core.Models;

namespace HtmlCompiler.Core.Interfaces;

public interface ITemplateManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="templateName"></param>
    /// <returns></returns>
    Task<IEnumerable<Template>> SearchTemplatesAsync(string templateName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <returns></returns>
    Task DownloadTemplateAsync(Template template);
}