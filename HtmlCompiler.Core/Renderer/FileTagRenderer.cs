using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core.Renderer;

public class FileTagRenderer : RenderingBase
{
    public const string RENDERER_TAG = @"@File=([^\s]+)";

    public FileTagRenderer(ILogger<FileTagRenderer> logger,
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
        Regex fileTagRegex = new Regex(RENDERER_TAG, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        foreach (Match match in fileTagRegex.Matches(content))
        {
            string fileValue = match.Groups[1].Value;

            string fullPath = Path.Combine(this._configuration.BaseDirectory, fileValue);

            // render the new file and return the rendered content
            string fileContent = await this._htmlRenderer.RenderHtmlAsync(fullPath,
                this._configuration.SourceDirectory,
                this._configuration.OutputDirectory,
                this._configuration.CssOutputFilePath,
                this._configuration.GlobalVariables,
                this._configuration.CallLevel);

            content = content.Replace(match.Value, fileContent);
        }

        return content;
    }
}