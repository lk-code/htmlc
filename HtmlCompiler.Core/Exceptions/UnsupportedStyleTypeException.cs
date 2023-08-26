namespace HtmlCompiler.Core.Exceptions;

public class UnsupportedStyleTypeException : Exception
{
    public UnsupportedStyleTypeException()
    {
    }

    public UnsupportedStyleTypeException(string message) : base(message)
    {
    }

    public UnsupportedStyleTypeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}