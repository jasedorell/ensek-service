using MeterReadings.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeterReadings.Data.Repositories
{
    /// <inheritdoc />
    public class AccountRepository : IAccountRepository
    {
        private readonly MeterReadingsDbContext _dbContext;

        public AccountRepository(MeterReadingsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public Task<bool> Exists(int accountId)
        {
            return _dbContext.Accounts.AnyAsync(a => a.Id == accountId);
        }
    }
}
