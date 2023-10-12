using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public class LayoutRenderer : RenderingBase
{
    public const string LAYOUT_TAG = "@Layout";
    public const string BODY_TAG = "@Body";

    public LayoutRenderer(RenderingConfiguration configuration,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            htmlRenderer)
    {
    }

    /// <inheritdoc />
    public override async Task<string> RenderAsync(string content)
    {
        string result = await this.RenderContentWithLayout(content);

        return result;
    }

    private async Task<string> RenderContentWithLayout(string content)
    {
        string baseDirectory = this._configuration.BaseDirectory;
        string layoutPlaceholder = $"{LAYOUT_TAG}=";

        int layoutIndex = content.IndexOf(layoutPlaceholder, StringComparison.Ordinal);
        if (layoutIndex == -1)
        {
            return content;
        }

        int layoutPathStart = layoutIndex + layoutPlaceholder.Length;
        int layoutPathEnd = content.IndexOf('\n', layoutPathStart);
        string layoutFilePath = content[layoutPathStart..layoutPathEnd].Trim();

        string fullPath = Path.Combine(baseDirectory, layoutFilePath);

        string layoutContent;
        try
        {
            layoutContent = await this.FileSystemService.FileReadAllTextAsync(fullPath);
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"Layout file not found or couldn't be read: {fullPath}", ex);
        }

        string layoutDirectoryPath = fullPath.GetBaseDirectory() + "/";
        string renderedLayoutContent = await this._htmlRenderer.RenderHtmlStringAsync(
            layoutContent,
            layoutDirectoryPath,
            this._configuration.SourceDirectory,
            this._configuration.OutputDirectory,
            this._configuration.CssOutputFilePath,
            this._configuration.GlobalVariables,
            0);

        string cleanedContent = string.Concat(
            content.AsSpan(0, layoutIndex),
            content.AsSpan(layoutPathEnd + 1)
            );

        int bodyIndex = renderedLayoutContent.IndexOf(BODY_TAG, StringComparison.Ordinal);
        if (bodyIndex == -1)
        {
            return cleanedContent;
        }

        string result = string.Concat(
            renderedLayoutContent.AsSpan(0, bodyIndex),
            cleanedContent,
            renderedLayoutContent.AsSpan(bodyIndex + BODY_TAG.Length)
            );

        return result;
    }
}