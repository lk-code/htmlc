using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime Now()
    {
        return DateTime.Now;
    }
}