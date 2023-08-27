namespace HtmlCompiler.Core.Exceptions;

public class StyleCommandNotFoundException : Exception
{
    public StyleCommandNotFoundException()
    {
    }

    public StyleCommandNotFoundException(string message) : base(message)
    {
    }

    public StyleCommandNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}