using System;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using DartSassHost;
using DartSassHost.Helpers;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using JavaScriptEngineSwitcher.V8;

namespace HtmlCompiler.Core;

public class StyleCompiler : IStyleCompiler
{
    private string _sourceDirectoryPath = null!;
    private string _outputDirectoryPath = null!;

    public StyleCompiler()
    {
    }

    public async Task<string?> CompileStyleAsync(string sourceDirectoryPath, string outputDirectoryPath, string? styleSourceFilePath)
    {
        this._sourceDirectoryPath = sourceDirectoryPath;
        this._outputDirectoryPath = outputDirectoryPath;

        if (string.IsNullOrEmpty(styleSourceFilePath))
        {
            return null;
        }

        Console.WriteLine($"looking for style at {styleSourceFilePath}");

        if (!File.Exists(styleSourceFilePath))
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
        switch (fileExtension)
        {
            case ".scss":
            case ".sass":
                {
                    await this.CompileSass(inputContent, sourceFilePath, outputFilePath);
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

    private async Task<string> GetStyleContent(string sourceDirectoryPath, string sourceFilePath)
    {
        string sourceFullFilePath = $"{sourceDirectoryPath}{sourceFilePath}";

        string content = await File.ReadAllTextAsync(sourceFullFilePath);
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
            content = await this.ReplaceSassImports(sourceDirectoryPath,
                content,
                currentSubDirectory,
                extension);
        }
        else if (extension == ".less")
        {
            // replace less imports
        }

        return content;
    }

    private async Task<string> ReplaceSassImports(string sourceDirectoryPath,
        string input,
        string currentSubDirectory,
        string fileExtension)
    {
        var importRegex = new Regex(@"@import\s+""([^""]+)""\s*;", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        foreach (Match match in importRegex.Matches(input))
        {
            string requestedSassFile = match.Groups[1].Value;
            requestedSassFile = requestedSassFile.GetFullObjectNameBySassImportName(fileExtension);

            requestedSassFile = Path.Combine(sourceDirectoryPath, currentSubDirectory, requestedSassFile);

            string replacement = await this.GetStyleContent(sourceDirectoryPath, requestedSassFile);

            input = input.Replace(match.Value, replacement);
        }

        return input;
    }

    private async Task CompileSass(string inputContent, string styleSourceFilePath, string styleOutputFilePath)
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
                    outputDirectory.EnsurePath();
                }

                string compiledCss = result.CompiledContent;
                await File.WriteAllTextAsync(outputFilePath, compiledCss);

                string mappingCss = result.SourceMap;
                await File.WriteAllTextAsync(outputMappingFilePath, mappingCss);

                Console.WriteLine($"compiled style to {outputFilePath} (mapping: {outputMappingFilePath})");

                //Console.WriteLine("Compiled content:{1}{1}{0}{1}", result.CompiledContent,
                //    Environment.NewLine);
                //Console.WriteLine("Source map:{1}{1}{0}{1}", result.SourceMap, Environment.NewLine);
                //Console.WriteLine("Included file paths: {0}",
                //    string.Join(", ", result.IncludedFilePaths));
            }
        }
        catch (SassCompilerLoadException e)
        {
            Console.WriteLine("During loading of Sass compiler an error occurred. See details:");
            Console.WriteLine();
            Console.WriteLine(SassErrorHelpers.GenerateErrorDetails(e));
        }
        catch (SassCompilationException e)
        {
            Console.WriteLine("During compilation of SCSS code an error occurred. See details:");
            Console.WriteLine();
            Console.WriteLine(SassErrorHelpers.GenerateErrorDetails(e));
        }
        catch (SassException e)
        {
            Console.WriteLine("During working of Sass compiler an unknown error occurred. See details:");
            Console.WriteLine();
            Console.WriteLine(SassErrorHelpers.GenerateErrorDetails(e));
        }
    }
}