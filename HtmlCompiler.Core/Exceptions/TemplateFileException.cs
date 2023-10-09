namespace HtmlCompiler.Core.Exceptions;

public class TemplateFileException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }
    
    public TemplateFileException()
    {
    }

    public TemplateFileException(string message) : base(message)
    {
    }

    public TemplateFileException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public TemplateFileException(IReadOnlyCollection<string> errors)
    {
        this.Errors = errors;
    }
}