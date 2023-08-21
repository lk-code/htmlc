using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;

namespace HtmlCompiler.Core.StyleRenderer;

public class ScssRenderer : IScssStyleRenderer
{
    /// <inheritdoc />
    public Task CompileStyle(string inputContent, string styleSourceFilePath, string styleOutputFilePath)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<StyleRenderingResult> Compile(string inputContent)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IEnumerable<string>> GetImports(string inputContent)
    {
        throw new NotImplementedException();
    }
}