using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.RenderingComponents;

public class FileTagRenderer : RenderingBase
{
    /// <inheritdoc />
    public override async Task<string> RenderAsync(string content)
    {
        Regex fileTagRegex = new Regex(@"@File=([^\s]+)", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        foreach (Match match in fileTagRegex.Matches(content))
        {
            string fileValue = match.Groups[1].Value;

            string fullPath = Path.Combine(this._configuration.BaseDirectory, fileValue);

            // render the new file and return the rendered content
            string fileContent = await this._htmlRenderer.RenderHtmlAsync(fullPath,
                this._configuration.SourceDirectory,
                this._configuration.OutputDirectory,
                this._configuration.CssOutputFilePath);

            content = content.Replace(match.Value, fileContent);
        }

        return content;
    }

    public FileTagRenderer(RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            fileSystemService,
            htmlRenderer)
    {
    }
}