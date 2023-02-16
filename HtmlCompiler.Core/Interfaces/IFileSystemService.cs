namespace HtmlCompiler.Core.Interfaces;

public interface IFileSystemService
{
    bool FileExists(string? path);
    Task<string> FileReadAllTextAsync(string path, CancellationToken cancellationToken = default(CancellationToken));
    Task FileWriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default(CancellationToken));
    void FileCopy(string sourceFileName, string destFileName, bool overwrite);
