using MeterReadings.Core.Models;
using MeterReadings.Core.Repositories;
using MeterReadings.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadings.Data.Repositories
{
    /// <inheritdoc />
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly MeterReadingsDbContext _dbContext;

        public MeterReadingRepository(MeterReadingsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task Save(MeterReadingEntry meterReadingEntry)
        {
            var (accountId, meterReadingDateTime, meterReadValue) = meterReadingEntry;

            _dbContext.MeterReadings.Add(new MeterReading
            {
                AccountId = accountId,
                MeterReadingDateTime = meterReadingDateTime,
                MeterReadValue = meterReadValue
            });

            await _dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MeterReadingEntry>> GetByAccountId(int accountId)
        {
            return await _dbContext.MeterReadings
                .Where(m => m.AccountId == accountId)
                .Select(m => new MeterReadingEntry(m.AccountId, m.MeterReadingDateTime, m.MeterReadValue))
                .ToListAsync();
        }
    }
}
