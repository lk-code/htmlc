namespace HtmlCompiler.Core.Components;

public class FileChangeDetector
{
    public event EventHandler<FileSystemEventArgs> FileChanged = null!;

    public FileChangeDetector(string path)
    {
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = path;
        watcher.Changed += OnFileChanged;
        watcher.Created += OnFileChanged;
        watcher.Deleted += OnFileChanged;
        watcher.Renamed += OnFileChanged;
        watcher.EnableRaisingEvents = true;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        FileChanged?.Invoke(this, e);
    }
}