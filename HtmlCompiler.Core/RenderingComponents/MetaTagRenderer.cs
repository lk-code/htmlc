using HtmlAgilityPack;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.RenderingComponents;

public class MetaTagRenderer : RenderingBase, IMetaTagRenderer
{
    public override async Task<string> RenderAsync(string content)
    {
        content = this.AddMetaTagToContent(content, "generator", "htmlc");

        return content;
    }

    public string AddMetaTagToContent(string html, string name, string content)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head == null)
        {
            var htmlNode = doc.DocumentNode.SelectSingleNode("html");
            head = doc.CreateElement("head");
            htmlNode?.AppendChild(head);
        }

        var metaTag = doc.CreateElement("meta");
        metaTag.SetAttributeValue("name", name);
        metaTag.SetAttributeValue("content", content);
        head.AppendChild(metaTag);

        return doc.DocumentNode.OuterHtml;
    }

    public MetaTagRenderer(RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            fileSystemService,
            htmlRenderer)
    {
    }
}