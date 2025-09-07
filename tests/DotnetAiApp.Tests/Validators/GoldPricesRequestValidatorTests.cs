using DotnetAiApp.Core.Utils;
using FluentValidation.TestHelper;
using DotnetAiApp.NbpApiClient.NbpApiClient;
using NSubstitute;

namespace DotnetAiApp.Tests.Validators
{
    public class GoldPricesRequestValidatorTests
    {
        private readonly ITimeProvider _timeProvider = Substitute.For<ITimeProvider>();

        private readonly GoldPricesRequestValidator _validator;

        public GoldPricesRequestValidatorTests()
        {
            _timeProvider.CurrentDate.Returns(new DateOnly(2023, 10, 1));
            _validator = new GoldPricesRequestValidator(_timeProvider);
        }

        record GetGoldPricesRequest(DateOnly? StartDate, DateOnly? EndDate) : IGetGoldPricesRequest;

        [Fact]
        public void NoStartDateProvided_HasValidationErrorForStartDate()
        {
            // Arrange
            var request = new GetGoldPricesRequest(null, null);

            // Act & Assert
            _validator.TestValidate(request)
                .ShouldHaveValidationErrorFor(x => x.StartDate);
        }

        [Fact]
        public void StartDateBeforeMinDate_HasValidationErrorForStartDate()
        {
            // Arrange
            var request = new GetGoldPricesRequest(new DateOnly(2012, 12, 31), null);

            // Act & Assert
            _validator.TestValidate(request)
                .ShouldHaveValidationErrorFor(x => x.StartDate);
        }

        [Fact]
        public void EndDateAfterCurrentDate_HasValidationErrorForEndDate()
        {
            // Arrange
            var request = new GetGoldPricesRequest(new DateOnly(2023, 10, 1), new DateOnly(2023, 10, 2));

            _timeProvider.CurrentDate.Returns(new DateOnly(2023, 10, 1));

            // Act & Assert
            _validator.TestValidate(request)
                .ShouldHaveValidationErrorFor(x => x.EndDate);
        }

        [Fact]
        public void EndDateBeforeStartDate_HasValidationErrorForEndDate()
        {
            // Arrange
            var request = new GetGoldPricesRequest(new DateOnly(2023, 10, 2), new DateOnly(2023, 10, 1));

            // Act & Assert
            _validator.TestValidate(request)
                .ShouldHaveValidationErrorFor(x => x.EndDate);
        }

        [Fact]
        public void DateSpanExceedsMaxSpanDays_HasValidationErrorForEndDate()
        {
            // Arrange
            var request = new GetGoldPricesRequest(new DateOnly(2023, 10, 1), new DateOnly(2024, 4, 1));

            // Act & Assert
            _validator.TestValidate(request)
                .ShouldHaveValidationErrorFor(x => x.EndDate);
        }

        [Fact]
        public void ValidDates_HasNoValidationErrors()
        {
            // Arrange
            var request = new GetGoldPricesRequest(new DateOnly(2023, 10, 1), new DateOnly(2023, 10, 1));

            // Act & Assert
            _validator.TestValidate(request)
                .ShouldNotHaveAnyValidationErrors();
        }
    }
}