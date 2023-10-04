using HtmlAgilityPack;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public class MetaTagRenderer : RenderingBase, IMetaTagRenderer
{
    public MetaTagRenderer(RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            fileSystemService,
            htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        await Task.CompletedTask;

        content = this.AddMetaTagToContent(content, "generator", "htmlc");

        return content;
    }

    public string AddMetaTagToContent(string html, string name, string content)
    {
        HtmlDocument doc = new();
        doc.LoadHtml(html);

        HtmlNode? head = doc.DocumentNode.SelectSingleNode("//head");
        if (head is null)
        {
            HtmlNode? htmlNode = doc.DocumentNode.SelectSingleNode("html");
            head = doc.CreateElement("head");
            htmlNode?.PrependChild(head);
            htmlNode?.PrependChild(HtmlNode.CreateNode("\n"));
            head.AppendChild(HtmlNode.CreateNode("\n"));
        }

        HtmlNode? existingMetaTag = head.SelectSingleNode($"meta[@name='{name}']");
        if (existingMetaTag is null)
        {
            // Wenn das Meta-Tag nicht existiert, erstelle ein neues
            HtmlNode? metaTag = doc.CreateElement("meta");
            metaTag.SetAttributeValue("name", name);
            head.AppendChild(metaTag);
            head.AppendChild(HtmlNode.CreateNode("\n"));
        }

        HtmlNode? updatedMetaTag = head.SelectSingleNode($"meta[@name='{name}']");
        updatedMetaTag?.SetAttributeValue("content", content);

        return doc.DocumentNode.OuterHtml;
    }
}