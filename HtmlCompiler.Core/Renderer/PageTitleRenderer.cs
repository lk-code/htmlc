using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public class PageTitleRenderer : RenderingBase
{
    public const string PAGETITLE_TAG = "@PageTitle";
    
    private static readonly Regex TitleDeclarationRegex = new Regex(PAGETITLE_TAG + @"=(.*?)(\r\n|\n|$)", RegexOptions.Compiled, TimeSpan.FromSeconds(10));
    private static readonly Regex TitleUseRegex = new Regex(PAGETITLE_TAG, RegexOptions.Compiled, TimeSpan.FromSeconds(10));

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