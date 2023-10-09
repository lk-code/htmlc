using System.IO.Compression;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core;

public class TemplatePackagingService : ITemplatePackagingService
{
    private readonly ILogger<TemplatePackagingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IFileSystemService _fileSystemService;

    public TemplatePackagingService(ILogger<TemplatePackagingService> logger,
        IConfiguration configuration,
        IFileSystemService fileSystemService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
    }

    public async Task CreateAsync(string sourcePath, string? outputPath = null)
    {
        if (string.IsNullOrEmpty(sourcePath))
        {
            throw new ArgumentException("invalid source path", nameof(sourcePath));
        }

        string fullSourcePath = Path.Combine(sourcePath, "src");
        string fullOutputFilePath = GetOutputFilePath(sourcePath, outputPath);

        IEnumerable<string> files = this._fileSystemService.GetAllFiles(fullSourcePath);

        if (this._fileSystemService.FileExists(fullOutputFilePath))
        {
            this._fileSystemService.Delete(fullOutputFilePath);
        }

        IReadOnlyCollection<string> errors = this.CreateZipFile(files, fullSourcePath, fullOutputFilePath);

        this._logger.LogInformation($"Template created at {fullOutputFilePath}.");

        if (errors.Count > 0)
        {
            throw new TemplateFileException(errors);
        }
    }

    private IReadOnlyCollection<string> CreateZipFile(IEnumerable<string> files, string rootDirectory, string outputFilePath)
    {
        List<string> errors = new();
        using ZipArchive zipArchive = ZipFile.Open(outputFilePath, ZipArchiveMode.Create);

        foreach (string file in files)
        {
            try
            {
                string relativePath = Path.GetRelativePath(rootDirectory, file);
                relativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/');

                ZipArchiveEntry entry = zipArchive.CreateEntry(relativePath);

                using Stream entryStream = entry.Open();
                using FileStream fileStream = File.OpenRead(file);

                fileStream.CopyTo(entryStream);

                this._logger.LogDebug("Add file {File}", file);
            }
            catch (Exception err)
            {
                this._logger.LogError(err, "An error occurred while adding file '{file}'", file);
                errors.Add($"An error occurred while adding file '{file}': {err.Message}");
            }
        }

        return errors;
    }

    private static string GetOutputFilePath(string sourcePath, string? outputPath = null)
    {
        if (string.IsNullOrEmpty(outputPath))
        {
            return Path.Combine(sourcePath, "template.zip");
        }
        else
        {
            string fileName = Path.GetFileName(outputPath);
            string extension = Path.GetExtension(fileName);

            if (!string.IsNullOrEmpty(fileName)
                && extension.Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    Path.GetFullPath(outputPath);
                    return outputPath;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"invalid output path: {ex.Message}", nameof(outputPath));
                }
            }
            else
            {
                throw new ArgumentException("invalid filetype or no filename given.", nameof(outputPath));
            }
        }
    }
}