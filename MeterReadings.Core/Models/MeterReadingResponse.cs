namespace MeterReadings.Core.Models
{
    /// <summary>
    /// Response details for a batch of meter readings that were processed.
    /// </summary>
    /// <param name="SuccessCount">The number of successful meter readings that were processed.</param>
    /// <param name="Errors">The <see cref="MeterReadingError"/> that occurred while the batch was processed.</param>
    public record MeterReadingResponse(int SuccessCount, ICollection<MeterReadingError> Errors);
}
