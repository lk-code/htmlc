using System;
namespace HtmlCompiler.Core.Components;

public class FileChangeDetector
{
    public event EventHandler<FileSystemEventArgs> FileChanged = null!;

    private FileSystemWatcher _watcher;

    public FileChangeDetector(string path)
    {
        _watcher = new FileSystemWatcher();
        _watcher.Path = path;
        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileChanged;
        _watcher.Deleted += OnFileChanged;
        _watcher.Renamed += OnFileChanged;
        _watcher.EnableRaisingEvents = true;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        FileChanged?.Invoke(this, e);
    }
}