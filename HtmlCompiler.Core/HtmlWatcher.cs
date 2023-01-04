using System;
using System.Globalization;
using System.Reflection.Metadata;
using HtmlCompiler.Core.Components;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class HtmlWatcher : IHtmlWatcher
{
    private readonly IHtmlRenderer _htmlRenderer;
    private readonly IStyleCompiler _styleCompiler;

    private string _sourceDirectoryPath = string.Empty;
    private string _outputDirectoryPath = string.Empty;
    private string? _styleEntryFilePath = null;
    private FileChangeDetector? _fileDetector = null;

    public HtmlWatcher(IHtmlRenderer htmlRenderer,
        IStyleCompiler styleCompiler)
    {
        this._htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
        this._styleCompiler = styleCompiler ?? throw new ArgumentNullException(nameof(styleCompiler));
    }

    ~HtmlWatcher()
    {
        this.UnregisterFileDetector();
    }

    private void UnregisterFileDetector()
    {
        if (this._fileDetector != null)
        {
            this._fileDetector.FileChanged -= FileChangeDetector_FileChanged;
        }
    }

    /// <inheritdoc/>
    public async Task WatchDirectoryAsync(string? sourcePath, string? outputPath, string? fileToStyleFilePath, bool watchDirectory = true)
    {
        Console.WriteLine($"htmlc is {((watchDirectory == true) ? "watching" : "compiling")} :)");

        // prepare
        this.SetProjectPaths(sourcePath, outputPath);

        this._sourceDirectoryPath.EnsurePath();
        this._outputDirectoryPath.EnsurePath();

        // check for style file
        if (!string.IsNullOrEmpty(fileToStyleFilePath))
        {
            string styleFullFilePath = $"{this._sourceDirectoryPath}{fileToStyleFilePath}";
            this._styleEntryFilePath = styleFullFilePath;
        }

        // compile files
        await this.CompileFilesAsync();

        // watch for changes
        if (watchDirectory == true)
        {
            this.UnregisterFileDetector();
            this._fileDetector = new FileChangeDetector(this._sourceDirectoryPath);
            this._fileDetector.FileChanged += FileChangeDetector_FileChanged;

            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("use CTRL+X to exit htmlc");
            Console.ForegroundColor = oldColor;
        }

        // loop => wait for user input to exit the app
        if (watchDirectory != true)
        {
            Console.WriteLine("compiling finished");
            return;
        }
        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                // CTRL+C doesnt work => use CTRL+X
                if (key.Modifiers == ConsoleModifiers.Control
                    && key.Key == ConsoleKey.X)
                {
                    Console.WriteLine("htmlc exited");
                    return;
                }
            }
        }
    }

    private async void FileChangeDetector_FileChanged(object? sender, FileSystemEventArgs e)
    {
        //string filePath = e.FullPath;
        //string fileExtension = Path.GetExtension(filePath);

        //Console.WriteLine($"file changing detected ({filePath})...");

        //switch (fileExtension.ToLower().Replace(".", ""))
        //{
        //    case "html":
        //        {
        //            await this.CompileFiles();
        //        }
        //        break;
        //}

        await this.CompileFilesAsync();
    }

    private async Task CompileFilesAsync()
    {
        Console.WriteLine($"compiling...");

        List<string> files = this._sourceDirectoryPath.GetAllFiles();

        // compile style file
        string? cssOutputFilePath = await this._styleCompiler.CompileStyleAsync(this._sourceDirectoryPath, this._outputDirectoryPath, this._styleEntryFilePath);

        // compile html
        await this.RenderHtmlFiles(files, cssOutputFilePath);
    }

    private async Task RenderHtmlFiles(List<string> files, string? cssOutputFilePath)
    {
        List<string> layoutFiles = this.GetLayoutFiles(files);
        List<string> sourceFiles = this.GetHtmlFiles(files);

        // get files
        //Console.WriteLine($"found {layoutFiles.Count()} layout files:");
        //foreach (string filePath in layoutFiles)
        //{
        //    Console.WriteLine(filePath);
        //}

        //Console.WriteLine($"found {sourceFiles.Count()} source files:");
        //foreach (string filePath in sourceFiles)
        //{
        //    Console.WriteLine(filePath);
        //}

        foreach (string sourceFile in sourceFiles)
        {
            string fileToCompile = sourceFile;
            string outputFile = this.GetOutputPathForSource(sourceFile, this._sourceDirectoryPath, this._outputDirectoryPath);

            string? outputDirectoryName = Path.GetDirectoryName(outputFile);
            if (!string.IsNullOrEmpty(outputDirectoryName))
            {
                outputDirectoryName.EnsurePath();
            }

            Console.WriteLine($"compile {fileToCompile} to {outputFile}");

            try
            {
                string renderedContent = await this._htmlRenderer.RenderHtmlAsync(fileToCompile, this._sourceDirectoryPath, this._outputDirectoryPath, cssOutputFilePath);

                await File.WriteAllTextAsync(outputFile, renderedContent);
            }
            catch (FileNotFoundException err)
            {
                Console.WriteLine($"ERR: file {err.FileName} not found");
            }
        }
    }

    internal string GetOutputPathForSource(string sourceFile, string projectPath, string outputPath)
    {
        string sourceFilePath = sourceFile.Replace(projectPath, "");
        string outputFilePath = $"{outputPath}{sourceFilePath}";

        return outputFilePath;
    }

    private List<string> GetHtmlFiles(List<string> files)
    {
        List<string> htmlFilePaths = files.Where(file => Path.GetExtension(file) == ".html")
            .ToList();

        return htmlFilePaths.Where(filePath => !Path.GetFileName(filePath).StartsWith("_"))
            .ToList();
    }

    private List<string> GetLayoutFiles(List<string> files)
    {
        List<string> htmlFiles = files.Where(file => Path.GetExtension(file) == ".html")
            .ToList();

        return htmlFiles.Where(filePath => Path.GetFileName(filePath).StartsWith("_"))
            .ToList();
    }

    private void SetProjectPaths(string? sourcePath, string? outputPath)
    {
        if (!string.IsNullOrEmpty(sourcePath)
            && !string.IsNullOrEmpty(outputPath))
        {
            // Source and destination directories were specified

            this._sourceDirectoryPath = sourcePath;
            this._outputDirectoryPath = outputPath;

            return;
        }
        else if (!string.IsNullOrEmpty(sourcePath)
            && string.IsNullOrEmpty(outputPath))
        {
            // Only one directory was specified.
            // Use as project directory =>/dist for output and /src for source

            this._sourceDirectoryPath = Path.Combine(sourcePath, "src");
            this._outputDirectoryPath = Path.Combine(sourcePath, "dist");

            return;
        }
        else
        {
            // No path was specified => use the current directory

            string baseDirectory = Directory.GetCurrentDirectory();
            this._sourceDirectoryPath = Path.Combine(baseDirectory, "src");
            this._outputDirectoryPath = Path.Combine(baseDirectory, "dist");

            return;
        }
    }
}