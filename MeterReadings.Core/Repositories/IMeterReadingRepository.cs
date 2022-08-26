using MeterReadings.Core.Models;

namespace MeterReadings.Core.Repositories
{
    /// <summary>
    /// Provides data access logic to the MeterReadings table.
    /// </summary>
    public interface IMeterReadingRepository
    {
        /// <summary>
        /// Saves a new entry in the MeterReadings table.
        /// </summary>
        /// <param name="meterReadingEntry">The meter reading to save.</param>
        Task Save(MeterReadingEntry meterReadingEntry);

        /// <summary>
        /// Retrieves all meter readings for the specified account id.
        /// </summary>
        /// <param name="accountId">The account id to retrieve meter readings for.</param>
        /// <returns>A list of <see cref="MeterReadingEntry"/>s for the account.</returns>
        Task<IReadOnlyCollection<MeterReadingEntry>> GetByAccountId(int accountId);
    }
}
