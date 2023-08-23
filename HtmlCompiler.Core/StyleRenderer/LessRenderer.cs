using System.Text.RegularExpressions;
using DartSassHost;
using DartSassHost.Helpers;
using dotless.Core;
using dotless.Core.configuration;
using HtmlCompiler.Core.Exceptions;
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
    public async Task<StyleRenderingResult> Compile(string inputContent)
    {
        await Task.CompletedTask;

        DotlessConfiguration options = new DotlessConfiguration();

        string styleResult = "";
        string mapResult = "";
        try
        {
            styleResult = Less.Parse(inputContent, options);
        }
        catch (Exception err)
        {
            throw new StyleException(err.Message, "", err);
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
        string importPattern = @"@import\s+((?:'(.*?)')|(?:""(.*?)""))\s*;";
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
                    .Trim());
            imports = imports.Concat(importsFromLine);
        }

        return imports;
    }
}