using System.Text.RegularExpressions;
using DartSassHost;
using DartSassHost.Helpers;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using JavaScriptEngineSwitcher.V8;

namespace HtmlCompiler.Core.StyleRenderer;

public class SassRenderer : ISassStyleRenderer
{
    private const string FILE_EXTENSION = "sass";

    private readonly IFileSystemService _fileSystemService;

    public SassRenderer(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetImports(string inputContent)
    {
        IEnumerable<string> imports = this.GetRawImports(inputContent);
        imports = imports.SelectMany(this.GetFullQualified)
            .ToList()
            .Distinct();

        return imports;
    }

    private IEnumerable<string> GetFullQualified(string import)
    {
        List<string> imports = new List<string>();

        string[] originalParts = import.Split(Path.DirectorySeparatorChar)
            .Reverse()
            .ToArray();
        string[] importParts = new List<string>(originalParts).ToArray();

        importParts[0] = $"{originalParts[0]}/";
        string directoryName = string.Join(Path.DirectorySeparatorChar, importParts.Reverse());
        imports.Add(directoryName);

        importParts[0] = $"{originalParts[0]}.{FILE_EXTENSION}";
        string mainName = string.Join(Path.DirectorySeparatorChar, importParts.Reverse());
        imports.Add(mainName);

        importParts[0] = $"_{originalParts[0]}.{FILE_EXTENSION}";
        string subName = string.Join(Path.DirectorySeparatorChar, importParts.Reverse());
        imports.Add(subName);

        return imports;
    }

    private IEnumerable<string> GetRawImports(string inputContent)
    {
        string importPattern = @"@import\s+(.*?)(?:\r?\n|$)";
        IEnumerable<string> imports = new List<string>();

        MatchCollection matches = Regex.Matches(inputContent, importPattern);
        foreach (Match match in matches)
        {
            string importLine = match.Value;
            importLine = importLine.Replace("@import", "")
                .TrimEnd()
                .TrimEnd(';')
                .Trim();

            IEnumerable<string> importsFromLine = importLine.Split(',')
                .Select(part => part.Trim()
                    .Trim('\'')
                    .Trim('\"')
                    .Trim('\r')
                    .Trim('\n')
                    .Trim());
            imports = imports.Concat(importsFromLine);
        }

        return imports;
    }
}