namespace HtmlCompiler.Core.Models;

public record Template
{
    public string Name { get; }
    public string FileName { get; }
    public string Url { get; }

    public Template(string name, string fileName, string url)
    {
        Name = name;
        FileName = fileName;
        Url = url;
    }
}