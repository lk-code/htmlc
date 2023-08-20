using DartSassHost;
using DartSassHost.Helpers;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using JavaScriptEngineSwitcher.V8;

namespace HtmlCompiler.Core.StyleRenderer;

public class SassRenderer : IStyleRenderer
{
    private readonly IFileSystemService _fileSystemService;

    public SassRenderer(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    public async Task CompileStyle(string inputContent, string styleSourceFilePath, string styleOutputFilePath)
    {
        CompilationOptions options = new CompilationOptions { SourceMap = true };

        string inputFilePath = styleSourceFilePath;
        string outputFilePath = styleOutputFilePath;
        string outputMappingFilePath = $"{styleOutputFilePath}.map";

        try
        {
            using (var sassCompiler = new SassCompiler(new V8JsEngineFactory()))
            {
                CompilationResult result = sassCompiler.Compile(
                    inputContent,
                    inputFilePath,
                    outputFilePath,
                    outputMappingFilePath,
                    options);

                string? outputDirectory = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(outputDirectory))
                {
                    this._fileSystemService.EnsurePath(outputDirectory);
                }

                string compiledCss = result.CompiledContent;
                await this._fileSystemService.FileWriteAllTextAsync(outputFilePath, compiledCss);

                string mappingCss = result.SourceMap;
                await this._fileSystemService.FileWriteAllTextAsync(outputMappingFilePath, mappingCss);

                Console.WriteLine($"compiled style to {outputFilePath} (mapping: {outputMappingFilePath})");
            }
        }
        catch (SassCompilerLoadException err)
        {
            throw new StyleException(err.Message, SassErrorHelpers.GenerateErrorDetails(err), err);
        }
        catch (SassCompilationException err)
        {
            throw new StyleException(err.Message, SassErrorHelpers.GenerateErrorDetails(err), err);
        }
        catch (SassException err)
        {
            throw new StyleException(err.Message, SassErrorHelpers.GenerateErrorDetails(err), err);
        }
    }

    public async Task<StyleRenderingResult> Compile(string inputContent)
    {
        await Task.CompletedTask;

        CompilationOptions options = new CompilationOptions { SourceMap = true };

        string styleResult = "";
        string mapResult = "";
        try
        {
            using (var sassCompiler = new SassCompiler(new V8JsEngineFactory()))
            {
                CompilationResult result = sassCompiler.Compile(
                    inputContent,
                    false,
                    options);
                
                styleResult = result.CompiledContent;
                mapResult = result.SourceMap;
            }
        }
        catch (SassCompilerLoadException err)
        {
            throw new StyleException(err.Message, SassErrorHelpers.GenerateErrorDetails(err), err);
        }
        catch (SassCompilationException err)
        {
            throw new StyleException(err.Message, SassErrorHelpers.GenerateErrorDetails(err), err);
        }
        catch (SassException err)
        {
            throw new StyleException(err.Message, SassErrorHelpers.GenerateErrorDetails(err), err);
        }

        return new StyleRenderingResult(styleResult,
            mapResult);
    }
}