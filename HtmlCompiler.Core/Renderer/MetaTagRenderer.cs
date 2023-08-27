using HtmlAgilityPack;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public class MetaTagRenderer : RenderingBase, IMetaTagRenderer
{
    public override async Task<string> RenderAsync(string content)
    {
        await Task.CompletedTask;
        
        content = this.AddMetaTagToContent(content, "generator", "htmlc");

        return content;
    }

    public string AddMetaTagToContent(string html, string name, string content)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        HtmlNode? head = doc.DocumentNode.SelectSingleNode("//head");
        if (head == null)
        {
            HtmlNode? htmlNode = doc.DocumentNode.SelectSingleNode("html");
            head = doc.CreateElement("head");
            htmlNode?.PrependChild(head);
            htmlNode?.PrependChild(HtmlNode.CreateNode("\n"));
            head.AppendChild(HtmlNode.CreateNode("\n"));
        }

        HtmlNode? metaTag = doc.CreateElement("meta");
        metaTag.SetAttributeValue("name", name);
        metaTag.SetAttributeValue("content", content);
        head.AppendChild(metaTag);
        head.AppendChild(HtmlNode.CreateNode("\n"));

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