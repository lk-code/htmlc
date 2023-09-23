using System.Text.RegularExpressions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core.Renderer;

public class StylePathRenderer : RenderingBase
{
    public const string STYLEPATH_TAG = "@StylePath";

    public StylePathRenderer(ILogger<FileTagRenderer> logger,
        RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(logger,
            configuration,
            fileSystemService,
            htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        await Task.CompletedTask;
        
        string cssOutputFilePath = this._configuration.CssOutputFilePath;
        string sourceFullFilePath = this._configuration.SourceFullFilePath;
        string sourceDirectory = this._configuration.SourceDirectory;
        string outputDirectory = this._configuration.OutputDirectory;
        
        if (!string.IsNullOrEmpty(cssOutputFilePath))
        {
            string entryFilePath = sourceFullFilePath.Replace(sourceDirectory, string.Empty);
            entryFilePath = $"{outputDirectory}{entryFilePath}";
            string relativeStylePath = entryFilePath.GetRelativePath(outputDirectory, cssOutputFilePath);

            content = ReplaceStylePath(content, relativeStylePath);
        }

        return content;
    }

    private static string ReplaceStylePath(string content,
        string cssPath)
    {
        Regex stylePathRegex = new Regex(STYLEPATH_TAG, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        return stylePathRegex.Replace(content, cssPath);
    }
}