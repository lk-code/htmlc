using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Core;

public class HtmlWatcher : IHtmlWatcher
{
    private readonly IConfiguration configuration;
    private readonly IHtmlRenderer _htmlRenderer;
    private readonly IStyleCompiler _styleCompiler;

    private string _sourceDirectoryPath = string.Empty;
    private string _outputDirectoryPath = string.Empty;
    private string? _styleEntryFilePath = null;
    private FileSystemWatcher? _fileSystemWatcher = null;
    private bool _watchDirectory = false;

    public HtmlWatcher(IConfiguration configuration,
        IHtmlRenderer htmlRenderer,
        IStyleCompiler styleCompiler)
    {
        this.configuration = configuration;
        this._htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
        this._styleCompiler = styleCompiler ?? throw new ArgumentNullException(nameof(styleCompiler));
    }

    ~HtmlWatcher()
    {
        this.UnregisterFileDetector();
    }

    private void UnregisterFileDetector()
    {
        if (this._fileSystemWatcher != null)
        {
            this._fileSystemWatcher.Changed -= FileSystemWatcher_Changed;
            this._fileSystemWatcher.Created -= FileSystemWatcher_Changed;
            this._fileSystemWatcher.Deleted -= FileSystemWatcher_Changed;
            this._fileSystemWatcher.Renamed -= FileSystemWatcher_Renamed;
        }
    }

    /// <inheritdoc/>
    public async Task WatchDirectoryAsync(string? sourcePath, string? outputPath, string? fileToStyleFilePath, bool watchDirectory = true)
    {
        this._watchDirectory = watchDirectory;
        Console.WriteLine($"htmlc is {((this._watchDirectory == true) ? "watching" : "compiling")} :)");

        try
        {
            this.ProcessDirectory(sourcePath, outputPath, fileToStyleFilePath);
        }
        catch (Exception err)
        {
            Console.WriteLine($"error: {err.Message}");

            if (this._watchDirectory != true)
            {
                return;
            }
        }

        // compile files
        await this.CompileFilesAsync();

        // watch for changes
        if (this._watchDirectory == true)
        {
            this.UnregisterFileDetector();
            this._fileSystemWatcher = new FileSystemWatcher();

            this._fileSystemWatcher.Path = this._sourceDirectoryPath;
            this._fileSystemWatcher.IncludeSubdirectories = true;

            this._fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;

            this._fileSystemWatcher.Filter = "*.*";

            this._fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            this._fileSystemWatcher.Created += FileSystemWatcher_Changed;
            this._fileSystemWatcher.Deleted += FileSystemWatcher_Changed;
            this._fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

            this._fileSystemWatcher.EnableRaisingEvents = true;

            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("use CTRL+X to exit htmlc");
            Console.ForegroundColor = oldColor;
        }

        // loop => wait for user input to exit the app
        if (this._watchDirectory != true)
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

    private void ProcessDirectory(string? sourcePath, string? outputPath, string? fileToStyleFilePath)
    {
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
    }

    private async void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
    {
        await this.CompileFilesAsync();
    }

    private async void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        await this.CompileFilesAsync();
    }

    private async Task CompileFilesAsync()
    {
        try
        {
            Console.WriteLine($"compiling...");

            List<string> files = this._sourceDirectoryPath.GetAllFiles();

            // compile style file
            string? cssOutputFilePath = await this._styleCompiler.CompileStyleAsync(this._sourceDirectoryPath, this._outputDirectoryPath, this._styleEntryFilePath);

            // compile html
            await this.RenderHtmlFiles(files, cssOutputFilePath);

            // copy additional assets (like js, css, images, etc.)
            this.CopyAssetsToOutput(files);
        }
        catch (Exception err)
        {
            Console.WriteLine($"error: {err.Message}");

            if (this._watchDirectory != true)
            {
                return;
            }
        }
    }

    private void CopyAssetsToOutput(List<string> sourceFiles)
    {
        // copy all other files from /src to /dist
        string[]? buildBlacklistArray = this.configuration.GetSection("BuildBlacklist").Get<string[]>();

        if (buildBlacklistArray == null
            || buildBlacklistArray.Length <= 0)
        {
            buildBlacklistArray = Array.Empty<string>();
        }

        List<string> buildBlacklist = buildBlacklistArray.ToList();
        if (!buildBlacklist.Contains(".html"))
        {
            buildBlacklist.Add(".html");
        }

        List<string> filesWithoutBlacklisted = sourceFiles
                .Where(x => buildBlacklist.Contains(Path.GetExtension(x.ToLowerInvariant())) != true)
                .ToList();

        // copy files to output
        foreach (string sourceFile in filesWithoutBlacklisted)
        {
            string outputFile = this.GetOutputPathForSource(sourceFile, this._sourceDirectoryPath, this._outputDirectoryPath);
            string outputDirectory = Path.GetDirectoryName(outputFile)!;
            outputDirectory.EnsurePath();

            // copy sourceFile to outputFile
            File.Copy(sourceFile, outputFile, true);
        }
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