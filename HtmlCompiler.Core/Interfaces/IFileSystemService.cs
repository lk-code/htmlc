namespace HtmlCompiler.Core.Interfaces;

public interface IFileSystemService
{
    bool FileExists(string? path);
    Task<string> FileReadAllTextAsync(string path);
    Task FileWriteAllTextAsync(string path, string? contents);
    void FileCopy(string sourceFileName, string destFileName, bool overwrite);
    bool DirectoryExists(string? path);
    void DirectoryCreate(string path);
    void EnsurePath(string path);
    string[] GetDirectories(string path);
    string[] GetFiles(string path);
    IEnumerable<string> GetAllFiles(string path);
    string GetCurrentDirectory();
    void Delete(string filePath);
}