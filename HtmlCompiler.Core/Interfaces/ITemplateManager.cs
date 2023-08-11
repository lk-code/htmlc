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
    /// <param name="template"></param>
    /// <returns></returns>
    Task<string> DownloadTemplateAsync(Template template);
}