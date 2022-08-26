using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MeterReadings.Core.Models;
using MeterReadings.Core.Repositories;
using MeterReadings.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MeterReadings.Core.Tests
{
    /// <summary>
    /// Verifies the behaviour of the <see cref="MeterReadingProcessor"/>.
    /// </summary>
    public class MeterReadingProcessorTests
    {
        private readonly Mock<ILogger<MeterReadingProcessor>> _mockLogger;
        private readonly Mock<IMeterReadingRepository> _mockMeterReadingRepository;
        private readonly Mock<IValidator<MeterReadingEntry>> _mockValidator;
        private readonly Fixture _fixture;

        public MeterReadingProcessorTests()
        {
            _mockLogger = new Mock<ILogger<MeterReadingProcessor>>();
            _mockMeterReadingRepository = new Mock<IMeterReadingRepository>();
            _mockValidator = new Mock<IValidator<MeterReadingEntry>>();
            _fixture = new Fixture();
        }

        /// <summary>
        /// Verifies that meter readings are saved when validation passes.
        /// </summary>
        [Fact]
        public async Task Process_SavesMeterReadings_WhenMeterReadingsAreValid()
        {
            var meterReadings = this.CreateMeterReadings();

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<MeterReadingEntry>(), new CancellationToken()))
                .ReturnsAsync(new ValidationResult());

            var savedMeterReadings = new List<MeterReadingEntry>();

            _mockMeterReadingRepository
                .Setup(r => r.Save(It.IsAny<MeterReadingEntry>()))
                .Callback<MeterReadingEntry>(m => savedMeterReadings.Add(m));

            var processor = this.CreateProcessor();

            await processor.Process(meterReadings);

            savedMeterReadings.Should().BeEquivalentTo(savedMeterReadings);
        }

        /// <summary>
        /// Verifies that process returns correct success count.
        /// </summary>
        [Fact]
        public async Task Process_ReturnsSuccessCount_WhenMeterIsProcessedSuccessfully()
        {
            var meterReadings = this.CreateMeterReadings();

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<MeterReadingEntry>(), new CancellationToken()))
                .ReturnsAsync(new ValidationResult());

            var processor = this.CreateProcessor();

            var response = await processor.Process(meterReadings);

            response.SuccessCount.Should().Be(meterReadings.Count());
            response.Errors.Should().BeEmpty();
        }

        /// <summary>
        /// Verifies that process returns a <see cref="MeterReadingError"/> when validation fails.
        /// </summary>
        [Fact]
        public async Task Process_ReturnsMeterReadingError_WhenValidationFails()
        {
            var invalidMeterReading = new MeterReadingEntry(_fixture.Create<int>(), DateTime.UtcNow.AddMonths(-1), "12345");
            const string validationErrorMessage = "AccountId does not exist";

            _mockValidator
                .Setup(v => v.ValidateAsync(invalidMeterReading, new CancellationToken()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new(nameof(MeterReadingEntry.AccountId), validationErrorMessage) }));

            var processor = this.CreateProcessor();

            var response = await processor.Process(new [] { invalidMeterReading });

            response.Errors.Should().ContainSingle();
            var (meterReadingEntry, validationErrors) = response.Errors.Single();
            meterReadingEntry.Should().Be(invalidMeterReading);
            validationErrors.Should().ContainSingle();
            var validationError = validationErrors.Single();
            validationError.Should().Contain(validationErrorMessage);
            response.SuccessCount.Should().Be(0);
        }

        /// <summary>
        /// Verifies that process returns a <see cref="MeterReadingError"/> for duplicate entries.
        /// </summary>
        [Fact]
        public async Task Process_ReturnsMeterReadingError_WhenDuplicateEntriesExist()
        {
            var accountId = _fixture.Create<int>();
            var meterReadingDateTime = DateTime.UtcNow.AddMonths(-1);
            const string meterReadingValue = "12345";

            var meterReadings = new[]
            {
                new MeterReadingEntry(accountId, meterReadingDateTime, meterReadingValue),
                new MeterReadingEntry(accountId, meterReadingDateTime, meterReadingValue)
            };

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<MeterReadingEntry>(), new CancellationToken()))
                .ReturnsAsync(new ValidationResult());

            var processor = this.CreateProcessor();

            var (_, meterReadingErrors) = await processor.Process(meterReadings);

            meterReadingErrors.Count.Should().Be(meterReadings.Length);
            var meterReadingFailures = new List<MeterReadingEntry>();
            foreach (var (meterReading, validationErrors) in meterReadingErrors)
            {
                validationErrors.Should().ContainSingle();
                var validationError = validationErrors.Single();
                validationError.Should().Be("Duplicate Entries are not allowed.");
                meterReadingFailures.Add(meterReading);
            }

            meterReadingFailures.Should().BeEquivalentTo(meterReadings);
        }

        /// <summary>
        /// Creates the <see cref="MeterReadingProcessor"/> with mock dependencies.
        /// </summary>
        /// <returns>The <see cref="MeterReadingProcessor"/>.</returns>
        private MeterReadingProcessor CreateProcessor()
        {
            return new MeterReadingProcessor(
                _mockLogger.Object,
                _mockMeterReadingRepository.Object,
                _mockValidator.Object);
        }

        /// <summary>
        /// Creates test <see cref="MeterReadingEntry"/>s.
        /// </summary>
        /// <returns>The <see cref="MeterReadingEntry"/>s.</returns>
        private IEnumerable<MeterReadingEntry> CreateMeterReadings()
        {
            return new[]
            {
                new MeterReadingEntry(_fixture.Create<int>(), DateTime.UtcNow.AddMonths(-1), "09812"),
                new MeterReadingEntry(_fixture.Create<int>(), DateTime.UtcNow.AddMonths(-2), "54895"),
                new MeterReadingEntry(_fixture.Create<int>(), DateTime.UtcNow.AddMonths(-3), "03256")
            };
        }
    }
}
