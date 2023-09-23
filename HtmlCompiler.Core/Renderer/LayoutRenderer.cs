using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core.Renderer;

public class LayoutRenderer : RenderingBase
{
    public const string LAYOUT_TAG = "@Layout";
    public const string BODY_TAG = "@Body";

    public LayoutRenderer(ILogger<LayoutRenderer> logger,
        RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(logger,
            configuration,
            fileSystemService,
            htmlRenderer)
    {
    }

    /// <inheritdoc />
    public override async Task<string> RenderAsync(string content)
    {
        string result = await this.RenderContentWithLayout(content);
        this.AdjustBaseDirectoryToLayoutFile(content);

        return result;
    }

    private void AdjustBaseDirectoryToLayoutFile(string content)
    {
        string baseDirectory = this._configuration.BaseDirectory;
        string? layoutPath = content.GetLayoutFilePath();
        if (string.IsNullOrEmpty(layoutPath))
        {
            return;
        }

        this._configuration.BaseDirectory = Path.Combine(baseDirectory, Path.GetDirectoryName(layoutPath)!);
    }

    private async Task<string> RenderContentWithLayout(string content)
    {
        string baseDirectory = this._configuration.BaseDirectory;
        string layoutPlaceholder = $"{LAYOUT_TAG}=";

        int layoutIndex = content.IndexOf(layoutPlaceholder);
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
            layoutContent = await this._fileSystemService.FileReadAllTextAsync(fullPath);
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"Layout file not found or couldn't be read: {fullPath}", ex);
        }

        string cleanedContent = string.Concat(
            content.AsSpan(0, layoutIndex),
            content.AsSpan(layoutPathEnd + 1)
            );

        int bodyIndex = layoutContent.IndexOf(BODY_TAG);
        if (bodyIndex == -1)
        {
            return cleanedContent;
        }

        string result = string.Concat(
            layoutContent.AsSpan(0, bodyIndex),
            cleanedContent,
            layoutContent.AsSpan(bodyIndex + BODY_TAG.Length)
            );

        return result;
    }
}