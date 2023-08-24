namespace HtmlCompiler.Core.Exceptions;

public class ConsoleExecutionException : Exception
{
    public ConsoleExecutionException()
    {
    }

    public ConsoleExecutionException(string message) : base(message)
    {
    }

    public ConsoleExecutionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}