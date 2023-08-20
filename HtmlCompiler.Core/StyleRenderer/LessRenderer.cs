using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;

namespace HtmlCompiler.Core.StyleRenderer;

public class LessRenderer : IStyleRenderer
{
    public Task CompileStyle(string inputContent, string styleSourceFilePath, string styleOutputFilePath)
    {
        throw new NotImplementedException();
    }

    public Task<StyleRenderingResult> Compile(string inputContent)
    {
        throw new NotImplementedException();
    }
}