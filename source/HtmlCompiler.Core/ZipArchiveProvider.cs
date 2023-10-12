using System.IO.Compression;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core;

public class ZipArchiveProvider : IZipArchiveProvider
{
    private readonly ILogger<ZipArchiveProvider> _logger;

    public ZipArchiveProvider(ILogger<ZipArchiveProvider> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> CreateZipFile(IEnumerable<string> files, string rootDirectory, string outputFilePath)
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
}