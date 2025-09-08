using System.Text.Json;
using DotnetAiApp.Core.Utils;
using FluentValidation;
using DotnetAiApp.Db;
using DotnetAiApp.Db.Entities;
using DotnetAiApp.NbpApiClient.NbpApiClient;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace DotnetAiApp.Web.Logic.Commands;

public class GetAndSaveGoldPrices
{
    public class Command : IRequest<GoldPriceResult>, IGetGoldPricesRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
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
        private readonly DotentAiAppContext _context;

        public Handler(INbpApiClient nbpApiClient,
            IFileProvider fileProvider,
            ITimeProvider timeProvider,
            DotentAiAppContext context)
        {
            _nbpApiClient = nbpApiClient;
            _fileProvider = fileProvider;
            _timeProvider = timeProvider;
            _context = context;
        }

        public async ValueTask<GoldPriceResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var goldPrices = await _nbpApiClient.GetGoldPricesAsync(request, cancellationToken);

            if (!goldPrices.Any())
            {
                return new GoldPriceResult
                {
                    ErrorMessage = "No gold prices found"
                };
            }

            await SavePricesToDb(request, goldPrices, cancellationToken);
            await SavePricesToJsonFile(goldPrices, cancellationToken);

            return new GoldPriceResult
            {
                StartDatePrice = goldPrices.First().Price,
                EndDatePrice = goldPrices.Last().Price,
                AveragePrice = goldPrices.Average(p => p.Price)
            };
        }

        private async Task SavePricesToDb(Command request, NpbPriceDto[] pricesDtos, CancellationToken cancellationToken)
        {
            var existing = await _context.GoldPrices
                .Where(gp => gp.Date >= request.StartDate && gp.Date <= request.EndDate)
                .ToListAsync(cancellationToken);

            foreach (var priceDto in pricesDtos)
            {
                var existingEntity = existing.FirstOrDefault(gp => gp.Date == priceDto.Date);
                if (existingEntity is not null)
                {
                    existingEntity.Price = (double)priceDto.Price;
                    continue;
                }

                var entity = new GoldPrice
                {
                    Date = priceDto.Date,
                    Price = (double)priceDto.Price
                };

                _context.Add(entity);
            }

            await _context.SaveChangesAsync(cancellationToken);
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