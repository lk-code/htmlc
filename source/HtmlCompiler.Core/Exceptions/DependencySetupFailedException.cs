namespace HtmlCompiler.Core.Exceptions;

public class DependencySetupFailedException : Exception
{
    public DependencySetupFailedException()
    {
    }

    public DependencySetupFailedException(string message) : base(message)
    {
    }

    public DependencySetupFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}