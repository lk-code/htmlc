using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Renderer;

public class CommentTagRenderer : RenderingBase
{
    public const string COMMENT_TAG = "@Comment";

    public CommentTagRenderer(RenderingConfiguration configuration,
        IFileSystemService fileSystemService,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            fileSystemService,
            htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        await Task.CompletedTask;
        
        string[] lines = content.Split(Environment.NewLine);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(COMMENT_TAG))
            {
                int commentIndex = lines[i].IndexOf(COMMENT_TAG);
                string comment = lines[i].Substring(commentIndex + COMMENT_TAG.Length);
                comment = comment.Trim().Replace("=", "");
                lines[i] = "<!-- " + comment + " -->";
            }
        }
        
        string result = string.Join(Environment.NewLine, lines);

        return result;
    }
}