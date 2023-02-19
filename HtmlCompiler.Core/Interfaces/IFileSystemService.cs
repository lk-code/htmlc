namespace HtmlCompiler.Core.Interfaces;

public interface IFileSystemService
{
    bool FileExists(string? path);
    Task<string> FileReadAllTextAsync(string path, CancellationToken cancellationToken = default(CancellationToken));
    Task FileWriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default(CancellationToken));
    void FileCopy(string sourceFileName, string destFileName, bool overwrite);
    bool DirectoryExists(string? path);
    void DirectoryCreate(string path);
    void EnsurePath(string path);
    string[] GetDirectories(string path);
    string[] GetFiles(string path);
    IEnumerable<string> GetAllFiles(string path);
    string GetCurrentDirectory();
}