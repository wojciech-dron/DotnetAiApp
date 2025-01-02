using FluentValidation;
using NbpApp.Utils.Utils;

namespace NbpApp.NbpApiClient.Validators;

public sealed class GoldPricesRequestValidator : AbstractValidator<IGetGoldPricesRequest>
{
    private const int MaxSpanDays = 180;
    private static readonly DateOnly MinDate = new(2013, 01, 01);

    public GoldPricesRequestValidator(ITimeProvider timeProvider)
    {
        RuleFor(c => c.StartDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .GreaterThan(MinDate)
            .LessThanOrEqualTo(timeProvider.CurrentDate);

        RuleFor(c => c.EndDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .GreaterThan(MinDate)
            .LessThanOrEqualTo(timeProvider.CurrentDate)
            .GreaterThanOrEqualTo(c => c.StartDate)
            .Must((command, endDate) => endDate!.Value.DayNumber - command.StartDate!.Value.DayNumber <= MaxSpanDays)
            .When(command => command.StartDate is not null)
            .WithMessage($"Span must be less than a {MaxSpanDays} days");
    }
}