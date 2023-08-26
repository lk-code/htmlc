using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Core;

public class StyleManager : IStyleManager
{
    private readonly IConfiguration _configuration;
    private readonly IFileSystemService _fileSystemService;

    private string _sourceDirectoryPath = null!;
    private string _outputDirectoryPath = null!;

    public StyleManager(IConfiguration configuration,
        IFileSystemService fileSystemService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    public async Task<string?> CompileStyleAsync(string sourceDirectoryPath, string outputDirectoryPath, string? styleSourceFilePath)
    {
        this._sourceDirectoryPath = sourceDirectoryPath;
        this._outputDirectoryPath = outputDirectoryPath;
        
        string blub = this._configuration.GetValue<string>("MEGA_TEST");

        if (string.IsNullOrEmpty(styleSourceFilePath))
        {
            return null;
        }

        Console.WriteLine($"looking for style at {styleSourceFilePath}");

        if(!this._fileSystemService.FileExists(styleSourceFilePath))
        {
            throw new StyleNotFoundException($"style file not found at {styleSourceFilePath}");
        }

        string fileExtension = Path.GetExtension(styleSourceFilePath)
            .ToLowerInvariant();
        fileExtension.IsSupportedStyleFileOrThrow();

        string sourceFileName = styleSourceFilePath.Replace(sourceDirectoryPath, "");
        string sourceFilePath = styleSourceFilePath;
        string outputFileName = Path.ChangeExtension(sourceFileName, "css");
        string outputFilePath = $"{this._outputDirectoryPath}{outputFileName}";

        // load complete style
        string inputContent = await this.GetStyleContent(this._sourceDirectoryPath, sourceFileName);

        // compile style
        switch (fileExtension)
        {
            case ".scss":
            case ".sass":
                {
                    // await this.CompileSass(inputContent, sourceFilePath, outputFilePath);
                }
                break;
            case ".less":
                {
                    // TODO: add less support
                }
                break;
        }

        return outputFilePath;
    }

    public async Task<string> GetStyleContent(string sourceDirectoryPath, string sourceFilePath)
    {
        string sourceFullFilePath = $"{sourceDirectoryPath}{sourceFilePath}";

        string content = await this._fileSystemService.FileReadAllTextAsync(sourceFullFilePath);
        string extension = Path.GetExtension(sourceFullFilePath)
            .ToLowerInvariant();
        string? currentSubDirectory = Path.GetDirectoryName(sourceFilePath);
        if (string.IsNullOrEmpty(currentSubDirectory))
        {
            currentSubDirectory = string.Empty;
        }

        if (extension == ".scss"
            || extension == ".sass")
        {
            // replace sass imports
            content = await content.ReplaceSassImports(this,
                sourceDirectoryPath,
                currentSubDirectory,
                extension);
        }
        else if (extension == ".less")
        {
            // replace less imports
        }

        return content;
    }
}