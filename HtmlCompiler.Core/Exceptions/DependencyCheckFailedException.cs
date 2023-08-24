namespace HtmlCompiler.Core.Exceptions;

public class DependencyCheckFailedException : Exception
{
    public DependencyCheckFailedException()
    {
    }

    public DependencyCheckFailedException(string message) : base(message)
    {
    }

    public DependencyCheckFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}