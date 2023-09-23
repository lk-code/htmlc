using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core.Renderer;

public class PageTitleRenderer : RenderingBase
{
    public const string PAGETITLE_TAG = "@PageTitle";
    
    private static readonly Regex TitleDeclarationRegex = new Regex(PAGETITLE_TAG + @"=(.*?)(\r\n|\n|$)", RegexOptions.Compiled, TimeSpan.FromSeconds(10));
    private static readonly Regex TitleUseRegex = new Regex(PAGETITLE_TAG, RegexOptions.Compiled, TimeSpan.FromSeconds(10));

    public PageTitleRenderer(ILogger<PageTitleRenderer> logger,
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
        
        string pageTitle = string.Empty;

        content = TitleDeclarationRegex.Replace(content, match =>
        {
            pageTitle = match.Groups[1].Value;
            return string.Empty;
        });

        content = TitleUseRegex.Replace(content, match => pageTitle);

        return content;
    }
}