using DartSassHost;
using DartSassHost.Helpers;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using HtmlCompiler.Core.StyleRenderer;

namespace HtmlCompiler.Core;

public class StyleManager : IStyleManager
{
    private readonly IFileSystemService _fileSystemService;
    private readonly SassRenderer _sassRenderer;
    private readonly LessRenderer _lessRenderer;

    private string _sourceDirectoryPath = null!;
    private string _outputDirectoryPath = null!;

    public StyleManager(IFileSystemService fileSystemService,
        SassRenderer sassRenderer,
        LessRenderer lessRenderer)
    {
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        _sassRenderer = sassRenderer ?? throw new ArgumentNullException(nameof(sassRenderer));
        _lessRenderer = lessRenderer ?? throw new ArgumentNullException(nameof(lessRenderer));
    }

    public async Task<string?> CompileStyleAsync(string sourceDirectoryPath, string outputDirectoryPath,
        string? styleSourceFilePath)
    {
        this._sourceDirectoryPath = sourceDirectoryPath;
        this._outputDirectoryPath = outputDirectoryPath;

        if (string.IsNullOrEmpty(styleSourceFilePath))
        {
            return null;
        }

        Console.WriteLine($"looking for style at {styleSourceFilePath}");

        if (!this._fileSystemService.FileExists(styleSourceFilePath))
        {
            // no style found
            Console.WriteLine("ERR: no style file found!");

            return null;
        }

        string fileExtension = Path.GetExtension(styleSourceFilePath)
            .ToLowerInvariant();
        if (!fileExtension.IsSupportedStyleFile())
        {
            // not supported style
            Console.WriteLine("ERR: style type is not supported (only scss and less is supported to compile)");

            return null;
        }

        string sourceFileName = styleSourceFilePath.Replace(sourceDirectoryPath, "");
        string sourceFilePath = styleSourceFilePath;
        string outputFileName = Path.ChangeExtension(sourceFileName, "css");
        string outputFilePath = $"{this._outputDirectoryPath}{outputFileName}";

        // load complete style
        string inputContent = await this.GetStyleContent(this._sourceDirectoryPath, sourceFileName);

        // compile style
        StyleRenderingResult? styleRenderingResult = null;

        try
        {
            switch (fileExtension)
            {
                case ".scss":
                case ".sass":
                {
                    // await this._sassRenderer.CompileStyle(inputContent, sourceFilePath, outputFilePath);
                    styleRenderingResult = await this._sassRenderer.Compile(inputContent);
                }
                    break;
                case ".less":
                {
                    styleRenderingResult = await this._lessRenderer.Compile(inputContent);
                }
                    break;
            }
        }
        catch (StyleException err)
        {
            Console.WriteLine("An error occurred during style compilation:");
            Console.WriteLine();
            Console.WriteLine(err.CompilerInformations);
        }

        if (styleRenderingResult is null)
        {
            return null;
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