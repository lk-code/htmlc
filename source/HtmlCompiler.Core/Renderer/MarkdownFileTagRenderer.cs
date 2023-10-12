using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;
using Markdig;

namespace HtmlCompiler.Core.Renderer;

public class MarkdownFileTagRenderer : RenderingBase
{
    private readonly MarkdownPipeline _pipeline;
    
    public const string RENDERER_TAG = @"@MarkdownFile=([^\s]+)";

    public MarkdownFileTagRenderer(RenderingConfiguration configuration,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            htmlRenderer)
    {
        _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }

    /// <inheritdoc />
    public override async Task<string> RenderAsync(string content)
    {
        Regex fileTagRegex = new(RENDERER_TAG, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        foreach (Match match in fileTagRegex.Matches(content))
        {
            string fileValue = match.Groups[1].Value;

            string fullPath = Path.Combine(this._configuration.BaseDirectory, fileValue);
            fullPath = Path.GetFullPath(fullPath);

            if (this.FileSystemService.FileExists(fullPath) == false)
            {
                continue;
            }

            // load content
            string markdownContent = await this.FileSystemService.FileReadAllTextAsync(fullPath);
            
            // render markdown to html
            string renderedMarkdownContent = Markdown.ToHtml(markdownContent, _pipeline);

            content = content.Replace(match.Value, renderedMarkdownContent);
        }

        return content;
    }
}