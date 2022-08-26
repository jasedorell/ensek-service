using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MeterReadings.Core.Models;
using MeterReadings.Core.Repositories;
using MeterReadings.Core.Validators;
using Moq;
using Xunit;

namespace MeterReadings.Core.Tests
{
    /// <summary>
    /// Verifies the behaviour of the <see cref="MeterReadingEntryValidator"/>.
    /// </summary>
    public class MeterReadingEntryValidatorTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IMeterReadingRepository> _mockMeterReadingRepository;
        private readonly Fixture _fixture;

        public MeterReadingEntryValidatorTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockMeterReadingRepository = new Mock<IMeterReadingRepository>();
            _fixture = new Fixture();
        }

        /// <summary>
        /// Verifies that validation fails when the Account Id is less than or equal to zero.
        /// </summary>
        /// <param name="accountId">The Account Id to validate.</param>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99999)]
        public async Task Validate_Fails_WhenAccountIdIsLessThanOrEqualToZero(int accountId)
        {
            var validator = this.CreateValidator();
            var meterReading = new MeterReadingEntry(accountId, DateTime.UtcNow, "99999");

            _mockMeterReadingRepository
                .Setup(r => r.GetByAccountId(meterReading.AccountId))
                .ReturnsAsync(new List<MeterReadingEntry>());

            var validationResult = await validator.ValidateAsync(meterReading);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().ContainSingle();
            var error = validationResult.Errors.Single();
            error.ErrorMessage.Should().Be("AccountId must be greater than 0");
        }

        /// <summary>
        /// Verifies that validation fails when the Account Id does not exist.
        /// </summary>
        [Fact]
        public async Task Validate_Fails_WhenAccountIdDoesNotExist()
        {
            var validator = this.CreateValidator();
            var meterReading = new MeterReadingEntry(_fixture.Create<int>(), DateTime.UtcNow, "99999");

            _mockMeterReadingRepository.Setup(r => r.GetByAccountId(meterReading.AccountId))
                .ReturnsAsync(new List<MeterReadingEntry>());

            _mockAccountRepository
                .Setup(r => r.Exists(meterReading.AccountId))
                .ReturnsAsync(false);

            var validationResult = await validator.ValidateAsync(meterReading);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().ContainSingle();
            var error = validationResult.Errors.Single();
            error.ErrorMessage.Should().Be("AccountId does not exist");
        }

        /// <summary>
        /// Verifies that validation fails when a newer meter reading exists.
        /// </summary>
        [Fact]
        public async Task Validate_Fails_WhenNewerMeterReadingExists()
        {
            var validator = this.CreateValidator();
            var accountId = _fixture.Create<int>();
            var meterReading = new MeterReadingEntry(accountId, DateTime.UtcNow.AddMonths(-3), "12345");

            var existingMeterReadings = new[]
            {
                new MeterReadingEntry(accountId, DateTime.UtcNow.AddMonths(-1), "12347"),
                new MeterReadingEntry(accountId, DateTime.UtcNow.AddMonths(-2), "12346"),
            };

            _mockMeterReadingRepository.Setup(r => r.GetByAccountId(meterReading.AccountId))
                .ReturnsAsync(existingMeterReadings);

            _mockAccountRepository
                .Setup(r => r.Exists(meterReading.AccountId))
                .ReturnsAsync(true);

            var validationResult = await validator.ValidateAsync(meterReading);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().ContainSingle();
            var error = validationResult.Errors.Single();
            error.ErrorMessage.Should().Be("Meter Reading DateTime must be newer than the latest meter reading");
        }

        /// <summary>
        /// Verifies that validation succeeds when a newer meter reading is entered.
        /// </summary>
        [Fact]
        public async Task Validate_Succeeds_WhenMeterReadingIsNewer()
        {
            var validator = this.CreateValidator();
            var accountId = _fixture.Create<int>();
            var meterReading = new MeterReadingEntry(accountId, DateTime.UtcNow.AddMonths(-1), "12345");

            var existingMeterReadings = new[]
            {
                new MeterReadingEntry(accountId, DateTime.UtcNow.AddMonths(-3), "12347"),
                new MeterReadingEntry(accountId, DateTime.UtcNow.AddMonths(-2), "12346"),
            };

            _mockMeterReadingRepository.Setup(r => r.GetByAccountId(meterReading.AccountId))
                .ReturnsAsync(existingMeterReadings);

            _mockAccountRepository
                .Setup(r => r.Exists(meterReading.AccountId))
                .ReturnsAsync(true);

            var validationResult = await validator.ValidateAsync(meterReading);

            validationResult.IsValid.Should().BeTrue();
        }

        /// <summary>
        /// Verifies that validation fails when the meter reading is empty.
        /// </summary>
        /// <param name="meterReadingValue">The meter reading to validate.</param>
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Validate_Fails_WhenMeterReadingIsEmpty(string meterReadingValue)
        {
            var validator = this.CreateValidator();
            var meterReading = new MeterReadingEntry(_fixture.Create<int>(), DateTime.UtcNow, meterReadingValue);

            _mockMeterReadingRepository.Setup(r => r.GetByAccountId(meterReading.AccountId))
                .ReturnsAsync(new List<MeterReadingEntry>());

            _mockAccountRepository
                .Setup(r => r.Exists(meterReading.AccountId))
                .ReturnsAsync(true);

            var validationResult = await validator.ValidateAsync(meterReading);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().ContainSingle();
            var error = validationResult.Errors.Single();
            error.ErrorMessage.Should().Be("MeterReadValue cannot be empty");
        }

        /// <summary>
        /// Verifies that validation fails when the meter reading is not in a valid format.
        /// </summary>
        /// <param name="meterReadingValue">The meter reading to validate.</param>
        [Theory]
        [InlineData("VOID")]
        [InlineData("0X765")]
        [InlineData("999999")]
        [InlineData("0")]
        [InlineData("1234")]
        [InlineData("-12345")]
        [InlineData("123456")]
        public async Task Validate_Fails_WhenMeterReadingIsNotValidFormat(string meterReadingValue)
        {
            var validator = this.CreateValidator();
            var meterReading = new MeterReadingEntry(_fixture.Create<int>(), DateTime.UtcNow, meterReadingValue);

            _mockMeterReadingRepository.Setup(r => r.GetByAccountId(meterReading.AccountId))
                .ReturnsAsync(new List<MeterReadingEntry>());

            _mockAccountRepository
                .Setup(r => r.Exists(meterReading.AccountId))
                .ReturnsAsync(true);

            var validationResult = await validator.ValidateAsync(meterReading);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().ContainSingle();
            var error = validationResult.Errors.Single();
            error.ErrorMessage.Should().Be("MeterReadValue must be in the format NNNNN");
        }

        /// <summary>
        /// Verifies that validation succeeds when the meter reading is in a valid format.
        /// </summary>
        /// <param name="meterReadingValue">The meter reading to validate.</param>
        [Theory]
        [InlineData("12345")]
        [InlineData("01234")]
        [InlineData("00234")]
        [InlineData("00034")]
        [InlineData("00004")]
        [InlineData("00000")]
        [InlineData("99999")]
        public async Task Validate_Succeeds_WhenMeterReadingIsInValidFormat(string meterReadingValue)
        {
            var validator = this.CreateValidator();
            var meterReading = new MeterReadingEntry(_fixture.Create<int>(), DateTime.UtcNow, meterReadingValue);

            _mockMeterReadingRepository.Setup(r => r.GetByAccountId(meterReading.AccountId))
                .ReturnsAsync(new List<MeterReadingEntry>());

            _mockAccountRepository
                .Setup(r => r.Exists(meterReading.AccountId))
                .ReturnsAsync(true);

            var validationResult = await validator.ValidateAsync(meterReading);

            validationResult.IsValid.Should().BeTrue();
        }

        /// <summary>
        /// Creates a <see cref="MeterReadingEntryValidator"/> with mock dependencies.
        /// </summary>
        /// <returns>The <see cref="MeterReadingEntryValidator"/>.</returns>
        private MeterReadingEntryValidator CreateValidator()
        {
            return new MeterReadingEntryValidator(
                _mockAccountRepository.Object,
                _mockMeterReadingRepository.Object);
        }
    }
}