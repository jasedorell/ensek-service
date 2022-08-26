namespace MeterReadings.Core.Repositories
{
    /// <summary>
    /// Provides data access logic to the Account table.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Determines if an account exists.
        /// </summary>
        /// <param name="accountId">The account id to query.</param>
        /// <returns>True if the account exists, otherwise false.</returns>
        Task<bool> Exists(int accountId);
    }
}
