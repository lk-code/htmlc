using System.Text.RegularExpressions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.RenderingComponents;

public class StylePathRenderer : RenderingBase
{
    public const string STYLEPATH_TAG = "@StylePath";
    
    public override short Order { get; } = 500;

    public override async Task<string> RenderAsync(string content)
    {
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

    private string ReplaceStylePath(string content,
        string cssPath)
    {
        Regex stylePathRegex = new Regex(STYLEPATH_TAG, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        return stylePathRegex.Replace(content, cssPath);
    }

    public StylePathRenderer(RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            fileSystemService,
            htmlRenderer)
    {
    }
}