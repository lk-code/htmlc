namespace HtmlCompiler.Core.Exceptions;

public class StyleException : Exception
{
    public string CompilerInformations { get; private set; } = string.Empty;

    public StyleException()
    {
    }

    public StyleException(string message)
        : base(message)
    {
    }

    public StyleException(string message,
        Exception innerException)
        : base(message,
            innerException)
    {
    }

    public StyleException(string message,
        string compilerInformations,
        Exception innerException)
        : base(message,
            innerException)
    {
        this.CompilerInformations = compilerInformations;
    }
}