namespace HtmlCompiler.Core.Exceptions;

public class StyleNotFoundException : Exception
{
    public StyleNotFoundException()
    {
    }

    public StyleNotFoundException(string message) : base(message)
    {
    }

    public StyleNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}