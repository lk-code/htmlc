namespace HtmlCompiler.Core.Exceptions;

public class TemplateFileException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }
    
    public TemplateFileException(IReadOnlyCollection<string> errors)
    {
        this.Errors = errors;
    }
}