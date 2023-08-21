using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;

namespace HtmlCompiler.Core.StyleRenderer;

public class LessRenderer : ILessStyleRenderer
{
    private const string FILE_EXTENSION = "less";

    private readonly IFileSystemService _fileSystemService;

    public LessRenderer(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

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