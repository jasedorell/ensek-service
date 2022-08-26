using FluentValidation;
using MeterReadings.Core.Models;
using MeterReadings.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace MeterReadings.Core.Services
{
    /// <summary>
    /// Provides functionality for processing new <see cref="MeterReadingEntry"/>s.
    /// </summary>
    public class MeterReadingProcessor
    {
        private readonly ILogger<MeterReadingProcessor> _logger;
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly IValidator<MeterReadingEntry> _validator;

        public MeterReadingProcessor(
            ILogger<MeterReadingProcessor> logger,
            IMeterReadingRepository meterReadingRepository,
            IValidator<MeterReadingEntry> validator)
        {
            _logger = logger;
            _meterReadingRepository = meterReadingRepository;
            _validator = validator;
        }

        /// <summary>
        /// Processes a batch of <see cref="MeterReadingEntry"/>s saving all valid entries into the database and
        /// rejects any meter readings that do not pass validation.
        /// </summary>
        /// <param name="meterReadings">The <see cref="MeterReadingEntry"/> to process.</param>
        /// <returns>A <see cref="MeterReadingResponse"/> for the batch.</returns>
        public async Task<MeterReadingResponse> Process(IEnumerable<MeterReadingEntry> meterReadings)
        {
            var duplicateEntries = meterReadings.GroupBy(entry => entry.GetHashCode())
                .Where(group => group.Count() > 1)
                .Select(group => group.First())
                .ToList();

            var errors = new List<MeterReadingError>();
            foreach (var meterReadingEntry in meterReadings)
            {
                try
                {
                    var validationResult = await _validator.ValidateAsync(meterReadingEntry);
                    if (!validationResult.IsValid)
                    {
                        var errorMessages = validationResult
                            .ToDictionary()
                            .Select(keyValue => $"Property: {keyValue.Key}, Message: {string.Join("|", keyValue.Value)}")
                            .ToList();

                        errors.Add(new MeterReadingError(meterReadingEntry, errorMessages));
                        continue;
                    }

                    if (duplicateEntries.Contains(meterReadingEntry))
                    {
                        errors.Add(new MeterReadingError(meterReadingEntry, new[] { "Duplicate Entries are not allowed." }));
                        continue;
                    }

                    await _meterReadingRepository.Save(meterReadingEntry);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unexpected error occurred processing Meter Reading: {meterReadingEntry}");
                }
            }

            return new MeterReadingResponse(meterReadings.Count() - errors.Count, errors);
        }
    }
}
