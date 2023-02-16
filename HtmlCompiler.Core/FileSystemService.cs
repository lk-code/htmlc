using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class FileSystemService : IFileSystemService
{
	public FileSystemService()
	{
	}

    public bool FileExists(string? path)
    {
        return File.Exists(path);
    }

    public Task<string> FileReadAllTextAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
    {
	    return File.ReadAllTextAsync(path, cancellationToken);
    }

    public Task FileWriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default(CancellationToken))
    {
	    return File.WriteAllTextAsync(path, contents, cancellationToken);
    }

    public void FileCopy(string sourceFileName, string destFileName, bool overwrite)
    {
	    File.Copy(sourceFileName, destFileName, overwrite);
    }
}