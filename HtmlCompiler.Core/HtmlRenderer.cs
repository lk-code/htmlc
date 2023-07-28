using System.Text.RegularExpressions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.RenderingComponents;

namespace HtmlCompiler.Core;

public class HtmlRenderer : IHtmlRenderer
{
    private List<Type> _renderingComponents = new List<Type>
    {
        typeof(LayoutRenderer),
        typeof(FileTagRenderer),
        typeof(CommentTagRenderer),
        typeof(HtmlEscapeBlockRenderer),
        typeof(StylePathRenderer)
    };

    private readonly IFileSystemService _fileSystemService;

    public HtmlRenderer(IFileSystemService fileSystemService)
    {
        this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    /// <inheritdoc />
    public async Task<string> RenderHtmlAsync(string sourceFullFilePath,
        string sourceDirectory,
        string outputDirectory,
        string? cssOutputFilePath)
    {
        sourceFullFilePath = Path.GetFullPath(sourceFullFilePath);
        string originalContent = await this._fileSystemService.FileReadAllTextAsync(sourceFullFilePath);


        
        
        // ############
        // NEW
        string renderedContent = string.Empty;
        RenderingConfiguration configuration = new RenderingConfiguration
        {
            BaseDirectory = sourceFullFilePath.GetBaseDirectory(),
            SourceDirectory = sourceDirectory,
            OutputDirectory = outputDirectory,
            CssOutputFilePath = cssOutputFilePath,
            SourceFullFilePath = sourceFullFilePath
        };
        List<IRenderingComponent> renderingComponents = this._renderingComponents.BuildRenderingComponents(
            configuration,
            this._fileSystemService,
            this);

        string masterOutput = originalContent;
        foreach (IRenderingComponent renderingComponent in renderingComponents.OrderBy(x => x.Order))
        {
            masterOutput = await renderingComponent.RenderAsync(masterOutput);

            int i = 0;
        }

        // NEW
        // ############

        renderedContent = masterOutput;
        
        
        // // replace all @File=...
        // renderedContent = await this.ReplaceFilePlaceholdersAsync(originalContent, baseDirectory, sourceDirectory,
        //     outputDirectory, cssOutputFilePath);
        //
        // // replace @Layout=...
        // renderedContent = await this.ReplaceLayoutPlaceholderAsync(renderedContent, baseDirectory);

        // // check if layout-file and source-html-file are on different directories => baseDirectory must be adjusted
        // baseDirectory = this.AdjustBaseDirectoryToLayoutFile(originalContent, baseDirectory);

        // replace all @File=...
        // renderedContent = await this.ReplaceFilePlaceholdersAsync(renderedContent, baseDirectory, sourceDirectory,
        //     outputDirectory, cssOutputFilePath);

        // // replace all @Comment=...
        // renderedContent = renderedContent.ReplaceCommentTags();

        // // replace all HTML Escape Blocks=...
        // renderedContent = RenderHtmlEscapeBlocks(renderedContent);

        // // replace all @StylePath
        // if (!string.IsNullOrEmpty(cssOutputFilePath))
        // {
        //     string entryFilePath = sourceFullFilePath.Replace(sourceDirectory, string.Empty);
        //     entryFilePath = $"{outputDirectory}{entryFilePath}";
        //     string relativeStylePath = entryFilePath.GetRelativePath(outputDirectory, cssOutputFilePath);
        //
        //     renderedContent = ReplaceStylePath(renderedContent, relativeStylePath);
        // }

        // replace @PageTitle=...
        renderedContent = await this.ReplacePageTitlePlaceholderAsync(renderedContent);

        // add meta-tag "generator"
        renderedContent = renderedContent.AddMetaTag("generator", "htmlc");

        return renderedContent;
    }

    // public static string RenderHtmlEscapeBlocks(string html)
    // {
    //     string startTag = "@StartHtmlSpecialChars";
    //     string endTag = "@EndHtmlSpecialChars";
    //     int startIndex = html.IndexOf(startTag);
    //
    //     while (startIndex != -1)
    //     {
    //         int endIndex = html.IndexOf(endTag, startIndex);
    //         if (endIndex == -1)
    //         {
    //             endIndex = html.Length;
    //         }
    //
    //         string textToEscape = html.Substring(startIndex + startTag.Length, endIndex - startIndex - startTag.Length);
    //         string escapedText = Regex.Replace(textToEscape, "[<>&\"']", m =>
    //         {
    //             switch (m.Value)
    //             {
    //                 case "<": return "&#60;";
    //                 case ">": return "&#62;";
    //                 case "&": return "&#38;";
    //                 case "\"": return "&#34;";
    //                 case "'": return "&#39;";
    //             }
    //
    //             return m.Value;
    //         }, RegexOptions.None, TimeSpan.FromMilliseconds(100));
    //         escapedText = escapedText.Replace("\n", "<br>\n");
    //         html = html.Remove(startIndex, endIndex - startIndex + endTag.Length).Insert(startIndex, escapedText);
    //
    //         startIndex = html.IndexOf(startTag, startIndex + escapedText.Length);
    //     }
    //
    //     return html;
    // }

    // private static string ReplaceStylePath(string content,
    //     string cssPath)
    // {
    //     Regex stylePathRegex = new Regex("@StylePath", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
    //
    //     return stylePathRegex.Replace(content, cssPath);
    // }

    // private string AdjustBaseDirectoryToLayoutFile(string content,
    //     string baseDirectory)
    // {
    //     string? layoutPath = content.GetLayoutFilePath();
    //     if (string.IsNullOrEmpty(layoutPath))
    //     {
    //         return baseDirectory;
    //     }
    //
    //     baseDirectory = Path.Combine(baseDirectory, Path.GetDirectoryName(layoutPath)!);
    //
    //     return baseDirectory;
    // }

    // private async Task<string> ReplaceFilePlaceholdersAsync(string content,
    //     string baseDirectory,
    //     string sourceDirectory,
    //     string outputDirectory,
    //     string? cssPath)
    // {
    //     Regex fileTagRegex = new Regex(@"@File=([^\s]+)", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
    //
    //     foreach (Match match in fileTagRegex.Matches(content))
    //     {
    //         string fileValue = match.Groups[1].Value;
    //
    //         string fullPath = Path.Combine(baseDirectory, fileValue);
    //
    //         // render the new file and return the rendered content
    //         string fileContent = await this.RenderHtmlAsync(fullPath, sourceDirectory, outputDirectory, cssPath);
    //
    //         content = content.Replace(match.Value, fileContent);
    //     }
    //
    //     return content;
    // }

    private static readonly Regex TitleDeclarationRegex =
        new Regex(@"@PageTitle=(.*?)(\r\n|\n|$)", RegexOptions.Compiled);

    private static readonly Regex TitleUseRegex = new Regex(@"@PageTitle", RegexOptions.Compiled);

    public async Task<string> ReplacePageTitlePlaceholderAsync(string content)
    {
        string pageTitle = string.Empty;

        // Find title declarations
        content = TitleDeclarationRegex.Replace(content, match =>
        {
            pageTitle = match.Groups[1].Value;
            return string.Empty; // Remove declaration
        });

        // Replace usages of the title
        content = TitleUseRegex.Replace(content, match => pageTitle);

        return content;
    }

    private async Task<string> ReplaceLayoutPlaceholderAsync(string content,
        string baseDirectory)
    {
        string layoutPlaceholder = "@Layout=";
        string bodyPlaceholder = "@Body";

        int layoutIndex = content.IndexOf(layoutPlaceholder);
        if (layoutIndex == -1)
        {
            // Kein @Layout-Platzhalter gefunden, gibt einfach den ursprünglichen Inhalt zurück.
            return content;
        }

        // Extrahiere den Pfad zur Layout-Datei.
        int layoutPathStart = layoutIndex + layoutPlaceholder.Length;
        int layoutPathEnd = content.IndexOf('\n', layoutPathStart);
        string layoutFilePath = content[layoutPathStart..layoutPathEnd].Trim();

        // Erstelle den vollständigen Pfad zur Layout-Datei.
        string fullPath = Path.Combine(baseDirectory, layoutFilePath);

        // Lade den Inhalt der Layout-Datei.
        string layoutContent;
        try
        {
            layoutContent = await this._fileSystemService.FileReadAllTextAsync(fullPath);
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"Layout file not found or couldn't be read: {fullPath}", ex);
        }

        // Entferne die Zeile mit dem @Layout-Platzhalter aus dem ersten Inhalt.
        string cleanedContent = content.Substring(0, layoutIndex) + content.Substring(layoutPathEnd + 1);

        // Suchen nach dem @Body-Platzhalter in der Layout-Datei und ersetzen ihn durch den bereinigten Inhalt.
        int bodyIndex = layoutContent.IndexOf(bodyPlaceholder);
        if (bodyIndex == -1)
        {
            // Kein @Body-Platzhalter gefunden, gibt den bereinigten Inhalt der Layout-Datei zurück.
            return cleanedContent;
        }

        // Ersetzen des @Body-Platzhalters durch den bereinigten Inhalt.
        string result = layoutContent.Substring(0, bodyIndex) + cleanedContent +
                        layoutContent.Substring(bodyIndex + bodyPlaceholder.Length);

        return result;
    }
}