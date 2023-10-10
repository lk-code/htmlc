namespace HtmlCompiler.Core.Interfaces;

public interface IDateTimeProvider
{
    /// <summary>
    /// Returns a DateTime representing the current date and time. The resolution of the returned value depends on the system timer.
    /// </summary>
    /// <returns></returns>
    DateTime Now();
}