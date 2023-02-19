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

    public Task<string> FileReadAllTextAsync(string path)
    {
	    return File.ReadAllTextAsync(path);
    }

    public Task FileWriteAllTextAsync(string path, string? contents)
    {
	    return File.WriteAllTextAsync(path, contents);
    }

    public void FileCopy(string sourceFileName, string destFileName, bool overwrite)
    {
	    File.Copy(sourceFileName, destFileName, overwrite);
    }

    public bool DirectoryExists(string? path)
    {
	    return Directory.Exists(path);
    }

    public void DirectoryCreate(string path)
    {
	    Directory.CreateDirectory(path);
    }

    public void EnsurePath(string path)
    {
	    if (!this.DirectoryExists(path))
	    {
		    this.DirectoryCreate(path);
	    }
    }

    public string[] GetDirectories(string path)
    {
	    return Directory.GetDirectories(path);
    }

    public string[] GetFiles(string path)
    {
	    return Directory.GetFiles(path);
    }

    public IEnumerable<string> GetAllFiles(string path)
    {
	    List<string> fileList = new List<string>();
	    string[] directories = Directory.GetDirectories(path);
	    fileList.AddRange(Directory.GetFiles(path));

	    foreach (string directory in directories)
	    {
		    fileList.AddRange(this.GetAllFiles(directory));
	    }

	    return fileList;
    }

    public string GetCurrentDirectory()
    {
	    return Directory.GetCurrentDirectory();
    }
}