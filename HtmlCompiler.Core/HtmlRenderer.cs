using System.Text.RegularExpressions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class HtmlRenderer : IHtmlRenderer
{
    private readonly IFileSystemService _fileSystemService;

    public HtmlRenderer(IFileSystemService fileSystemService)
    {
        this._fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }
    
    public async Task<string> RenderHtmlAsync(string sourceFullFilePath,
        string sourceDirectory,
        string outputDirectory,
        string? cssOutputFilePath)
    {
        sourceFullFilePath = Path.GetFullPath(sourceFullFilePath);
        string baseDirectory = GetBaseDirectory(sourceFullFilePath);
        string originalContent = await LoadFileContent(sourceFullFilePath);
        string renderedContent = string.Empty;

        // replace all @File=...
        renderedContent = await this.ReplaceFilePlaceholdersAsync(originalContent, baseDirectory, sourceDirectory, outputDirectory, cssOutputFilePath);

        // replace @Layout=...
        renderedContent = await this.ReplaceLayoutPlaceholderAsync(renderedContent, baseDirectory);

        // check if layout-file and source-html-file are on different directories => baseDirectory must be adjusted
        baseDirectory = this.AdjustBaseDirectoryToLayoutFile(originalContent, baseDirectory);

        // replace all @File=...
        renderedContent = await this.ReplaceFilePlaceholdersAsync(renderedContent, baseDirectory, sourceDirectory, outputDirectory, cssOutputFilePath);

        // replace all @Comment=...
        renderedContent = renderedContent.ReplaceCommentTags();

        // replace all @Comment=...
        renderedContent = RenderHtmlEscapeBlocks(renderedContent);

        // replace all @StylePath
        if (!string.IsNullOrEmpty(cssOutputFilePath))
        {
            string entryFilePath = sourceFullFilePath.Replace(sourceDirectory, string.Empty);
            entryFilePath = $"{outputDirectory}{entryFilePath}";
            string relativeStylePath = entryFilePath.GetRelativePath(outputDirectory, cssOutputFilePath);

            renderedContent = ReplaceStylePath(renderedContent, relativeStylePath);
        }

        // add meta-tag "generator"
        renderedContent = renderedContent.AddMetaTag("generator", "htmlc");

        return renderedContent;
    }

    public static string RenderHtmlEscapeBlocks(string html)
    {
        string startTag = "@StartHtmlSpecialChars";
        string endTag = "@EndHtmlSpecialChars";
        int startIndex = html.IndexOf(startTag);

        while (startIndex != -1)
        {
            int endIndex = html.IndexOf(endTag, startIndex);
            if (endIndex == -1)
            {
                endIndex = html.Length;
            }
            string textToEscape = html.Substring(startIndex + startTag.Length, endIndex - startIndex - startTag.Length);
            string escapedText = Regex.Replace(textToEscape, "[<>&\"']", m =>
            {
                switch (m.Value)
                {
                    case "<": return "&#60;";
                    case ">": return "&#62;";
                    case "&": return "&#38;";
                    case "\"": return "&#34;";
                    case "'": return "&#39;";
                }
                return m.Value;
            });
            escapedText = escapedText.Replace("\n", "<br>\n");
            html = html.Remove(startIndex, endIndex - startIndex + endTag.Length).Insert(startIndex, escapedText);

            startIndex = html.IndexOf(startTag, startIndex + escapedText.Length);
        }
        return html;
    }

    private static string ReplaceStylePath(string content,
        string cssPath)
    {
        Regex stylePathRegex = new Regex("@StylePath", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        return stylePathRegex.Replace(content, cssPath);
    }

    private string AdjustBaseDirectoryToLayoutFile(string content,
        string baseDirectory)
    {
        string? layoutPath = GetLayoutFilePath(content);
        if (string.IsNullOrEmpty(layoutPath))
        {
            return baseDirectory;
        }

        baseDirectory = Path.Combine(baseDirectory, Path.GetDirectoryName(layoutPath)!);

        return baseDirectory;
    }

    private async Task<string> ReplaceFilePlaceholdersAsync(string content,
        string baseDirectory,
        string sourceDirectory,
        string outputDirectory,
        string? cssPath)
    {
        Regex fileTagRegex = new Regex(@"@File=([^\s]+)", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        foreach (Match match in fileTagRegex.Matches(content))
        {
            string fileValue = match.Groups[1].Value;

            string fullPath = Path.Combine(baseDirectory, fileValue);

            // render the new file and return the rendered content
            string fileContent = await this.RenderHtmlAsync(fullPath, sourceDirectory, outputDirectory, cssPath);

            content = content.Replace(match.Value, fileContent);
        }

        return content;
    }

    private static string? GetLayoutFilePath(string content)
    {
        var layoutRegex = new Regex("@Layout", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        Match layoutMatch = layoutRegex.Match(content);
        if (!layoutMatch.Success)
        {
            return null;
        }

        int lineBreakIndex = content.IndexOf(Environment.NewLine, layoutMatch.Index);
        if (lineBreakIndex < 0)
        {
            return null;
        }

        string layoutPath = content.Substring(layoutMatch.Index + 8, lineBreakIndex - layoutMatch.Index - 8).Trim();

        return layoutPath;
    }

    private async Task<string> ReplaceLayoutPlaceholderAsync(string content,
        string baseDirectory)
    {
        string? layoutPath = GetLayoutFilePath(content);
        if (string.IsNullOrEmpty(layoutPath))
        {
            return content;
        }

        string fullPath = Path.Combine(baseDirectory, layoutPath);
        string layoutContent = await this._fileSystemService.FileReadAllTextAsync(fullPath);
        layoutContent = layoutContent.Replace("@Body", content);

        string output = string.Join(Environment.NewLine, layoutContent.Split(Environment.NewLine)
            .Where(x => x.Trim().ToLowerInvariant().StartsWith("@layout"))
            .ToArray());

        return output;
    }

    private static async Task<string> LoadFileContent(string sourceFile)
    {
        string content = string.Empty;

        using (StreamReader streamReader = new StreamReader(sourceFile))
        {
            content = await streamReader.ReadToEndAsync();
        }

        return content;
    }

    private static string GetBaseDirectory(string sourceFile)
    {
        string? baseDirectory = Path.GetDirectoryName(sourceFile);

        if (string.IsNullOrEmpty(baseDirectory))
        {
            throw new FileNotFoundException($"\"{nameof(baseDirectory)}\" cannot be NULL or empty.", nameof(baseDirectory));
        }

        return baseDirectory;
    }
}