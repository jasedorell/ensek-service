using FluentValidation;
using MeterReadings.Core.Models;
using MeterReadings.Core.Repositories;

namespace MeterReadings.Core.Validators
{
    /// <summary>
    /// Defines validation rules for the <see cref="MeterReadingEntry"/> record.
    /// </summary>
    public class MeterReadingEntryValidator : AbstractValidator<MeterReadingEntry>
    {
        private readonly IMeterReadingRepository _meterReadingRepository;

        public MeterReadingEntryValidator(
            IAccountRepository accountRepository,
            IMeterReadingRepository meterReadingRepository)
        {
            _meterReadingRepository = meterReadingRepository;

            this.RuleFor(r => r.AccountId)
                .GreaterThan(0)
                .WithMessage("AccountId must be greater than 0")
                .MustAsync(((accountId, _) => accountRepository.Exists(accountId)))
                .When(m => m.AccountId > 0, ApplyConditionTo.CurrentValidator)
                .WithMessage("AccountId does not exist");

            this.RuleFor(r => r.MeterReadingDateTime)
                .MustAsync((e, _, _) => this.IsNewerEntry(e))
                .WithMessage("Meter Reading DateTime must be newer than the latest meter reading");

            this.RuleFor(r => r.MeterReadValue)
                .NotEmpty()
                .WithMessage("MeterReadValue cannot be empty")
                .Matches(@"^\d{5}$")
                .WithMessage("MeterReadValue must be in the format NNNNN")
                .When(m => !string.IsNullOrWhiteSpace(m.MeterReadValue), ApplyConditionTo.CurrentValidator);
        }

        /// <summary>
        /// Determines if the specified <see cref="MeterReadingEntry"/> is newer than previous readings
        /// for the specified account.
        /// </summary>
        /// <param name="meterReadingEntry">The <see cref="MeterReadingEntry"/> to validate.</param>
        /// <returns>True if the meter reading is newer, otherwise False.</returns>
        private async Task<bool> IsNewerEntry(MeterReadingEntry meterReadingEntry)
        {
            var latestReading = (await _meterReadingRepository
                    .GetByAccountId(meterReadingEntry.AccountId))
                .OrderByDescending(e => e.MeterReadingDateTime)
                .FirstOrDefault();

            if (latestReading == null)
            {
                return true;
            }

            return meterReadingEntry.MeterReadingDateTime > latestReading.MeterReadingDateTime;
        }
    }
}
