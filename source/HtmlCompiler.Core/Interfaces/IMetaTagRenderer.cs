namespace HtmlCompiler.Core.Interfaces;

public interface IMetaTagRenderer
{
    string AddMetaTagToContent(string html, string name, string content);
}