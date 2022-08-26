using FluentValidation.Results;

namespace MeterReadings.Core.Models
{
    /// <summary>
    /// Contains information for a meter reading that could not be processed because of an error.
    /// </summary>
    /// <param name="MeterReadingEntry">The <see cref="MeterReadingEntry"/> that couldn't be processed.</param>
    /// <param name="ValidationErrors">The validation errors that occurred.</param>
    public record MeterReadingError(MeterReadingEntry MeterReadingEntry, ICollection<string> ValidationErrors);
}
