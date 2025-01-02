using System.Text.Json;
using EFCore.BulkExtensions;
using FluentValidation;
using MediatR;
using NbpApp.Db;
using NbpApp.Db.Entities;
using NbpApp.NbpApiClient;
using NbpApp.NbpApiClient.Contracts;
using NbpApp.NbpApiClient.Validators;
using NbpApp.Utils.Utils;

namespace NbpApp.Web.Logic;

public class GetAndSaveGoldPrices
{
    public class Command : IRequest<GoldPriceResult>, IGetGoldPricesRequest
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(GoldPricesRequestValidator goldPricesRequestValidator)
        {
            Include(goldPricesRequestValidator);
        }
    }

    internal class Handler : IRequestHandler<Command, GoldPriceResult>
    {
        private readonly INbpApiClient _nbpApiClient;
        private readonly IFileProvider _fileProvider;
        private readonly ITimeProvider _timeProvider;
        private readonly NbpAppContext _context;

        public Handler(INbpApiClient nbpApiClient,
            IFileProvider fileProvider,
            ITimeProvider timeProvider,
            NbpAppContext context)
        {
            _nbpApiClient = nbpApiClient;
            _fileProvider = fileProvider;
            _timeProvider = timeProvider;
            _context = context;
        }

        public async Task<GoldPriceResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var goldPrices = await _nbpApiClient.GetGoldPricesAsync(request, cancellationToken);

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

            await _context.BulkInsertOrUpdateAsync(entities, cancellationToken: cancellationToken);
        }

        private async Task SavePricesToJsonFile(NpbPriceDto[] goldPrices, CancellationToken cancellationToken)
        {
            var fileName = $"gold_prices_request_{_timeProvider.CurrentTime:yyyy-MM-dd_HH-mm-ss}.json";

            var json = JsonSerializer.Serialize(goldPrices.Select(p => new
            {
                date = p.Date,
                price = p.Price
            }));

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