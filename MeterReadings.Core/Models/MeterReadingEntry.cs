namespace MeterReadings.Core.Models
{
    /// <summary>
    /// Contains all information needed to submit a new meter reading.
    /// </summary>
    /// <param name="AccountId">The account id associated with the meter reading.</param>
    /// <param name="MeterReadingDateTime">The date and time the meter reading was taken.</param>
    /// <param name="MeterReadValue">The meter reading value.</param>
    public record MeterReadingEntry(int AccountId, DateTime MeterReadingDateTime, string MeterReadValue);
}
