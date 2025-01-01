using System.Text.Json;
using FluentValidation;
using MediatR;
using NbpApp.Db.Entities;
using NbpApp.Db.Repos;
using NbpApp.NbpApiClient;
using NbpApp.NbpApiClient.Contracts;
using NbpApp.Utils.Utils;

namespace NbpApp.Web.Logic;

public class GetAndSaveGoldPrices
{
    public record Command(DateOnly StartDate, DateOnly EndDate) : IRequest<GoldPriceResult>;

    public class Validator : AbstractValidator<Command>
    {
        private const int MaxSpanDays = 180;
        private static readonly DateOnly MinDate = new(2013, 01, 01);
        public Validator(ITimeProvider timeProvider)
        {
            RuleFor(c => c.EndDate)
                .Cascade(CascadeMode.Stop)
                .Must(date => date <= timeProvider.CurrentDate)
                .WithMessage("End date must be less or equal now");

            RuleFor(c => c.StartDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .LessThan(MinDate)
                .WithMessage($"Start date must be greater than {MinDate:yyyy-MM-dd}")
                .Must((command, startDate) => startDate <= command.EndDate)
                .WithMessage("Start date must be less than end date")
                .Must((command, startDate) => startDate.DayNumber - command.EndDate.DayNumber <= MaxSpanDays)
                .WithMessage($"Span must be less than a {MaxSpanDays} days");
        }
    }

    class Handler : IRequestHandler<Command, GoldPriceResult>
    {
        private readonly INbpApiClient _nbpApiClient;
        private readonly IGoldPriceRepository _goldRepo;
        private readonly IFileProvider _fileProvider;

        public Handler(INbpApiClient nbpApiClient,
            IGoldPriceRepository goldRepo,
            IFileProvider fileProvider)
        {
            _nbpApiClient = nbpApiClient;
            _goldRepo = goldRepo;
            _fileProvider = fileProvider;
        }

        public async Task<GoldPriceResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var goldPrices = await _nbpApiClient.GetGoldPricesAsync(
                request.StartDate,
                request.EndDate,
                cancellationToken);

            if (!goldPrices.Any())
            {
                return new GoldPriceResult
                {
                    ErrorMessage = "No gold prices found"
                };
            }

            await SavePricesToDb(goldPrices, cancellationToken);
            await SavePricesToJsonFile(goldPrices, cancellationToken);

            return new GoldPriceResult
            {
                StartDatePrice = goldPrices.First().Price,
                EndDatePrice = goldPrices.Last().Price,
                AveragePrice = goldPrices.Average(p => p.Price)
            };
        }

        private async Task SavePricesToDb(NpbPriceDto[] pricesDtos, CancellationToken cancellationToken)
        {
            var entities = pricesDtos.Select(gp => new GoldPrice
            {
                Date = gp.Date,
                Price = gp.Price
            });

            await _goldRepo.AddOrUpdatePrices(entities, cancellationToken);
        }

        private async Task SavePricesToJsonFile(NpbPriceDto[] goldPrices, CancellationToken cancellationToken)
        {
            var fileName = $"gold_prices_request_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.json";
            var json = JsonSerializer.Serialize(goldPrices);

            await _fileProvider.WriteTextAsync(fileName, json, cancellationToken);
        }
    }
}

public class GoldPriceResult : AppResult
{
    public decimal StartDatePrice { get; init; }
    public decimal EndDatePrice { get; init; }
    public decimal AveragePrice { get; init; }
}