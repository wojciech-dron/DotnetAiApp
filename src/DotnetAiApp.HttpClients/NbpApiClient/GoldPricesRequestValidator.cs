using DotnetAiApp.Core.Utils;
using FluentValidation;

namespace DotnetAiApp.NbpApiClient.NbpApiClient;

public sealed class GoldPricesRequestValidator : AbstractValidator<IGetGoldPricesRequest>
{
    private const int MaxSpanDays = 180;
    private static readonly DateTime MinDate = new(2013, 01, 01);

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
            .Must((command, endDate) => (endDate!.Value - command.StartDate!.Value).Days <= MaxSpanDays)
            .When(command => command.StartDate is not null)
            .WithMessage($"Span must be less than a {MaxSpanDays} days");
    }
}