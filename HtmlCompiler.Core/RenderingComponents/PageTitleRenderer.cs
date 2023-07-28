using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.RenderingComponents;

public class PageTitleRenderer : RenderingBase
{
    private static readonly Regex TitleDeclarationRegex =
        new Regex(@"@PageTitle=(.*?)(\r\n|\n|$)", RegexOptions.Compiled);

    private static readonly Regex TitleUseRegex = new Regex(@"@PageTitle", RegexOptions.Compiled);

    public override short Order { get; } = 600;

    public override async Task<string> RenderAsync(string content)
    {
        string pageTitle = string.Empty;

        content = TitleDeclarationRegex.Replace(content, match =>
        {
            pageTitle = match.Groups[1].Value;
            return string.Empty;
        });

        content = TitleUseRegex.Replace(content, match => pageTitle);

        return content;
    }

    public PageTitleRenderer(RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            fileSystemService,
            htmlRenderer)
    {
    }
}