using MediatR;
using NbpApp.Db.Entities;
using NbpApp.Db.Repos;
using NbpApp.NbpApiClient;
using NbpApp.NbpApiClient.Contracts;
using NbpApp.Web.Common;

namespace NbpApp.Web.Logic;

public class GetAndSaveGoldPrices
{
    public record Command(DateOnly StartDate, DateOnly EndDate) : IRequest<GoldPriceResult>;

    class Handler : IRequestHandler<Command, GoldPriceResult>
    {
        private readonly INbpApiClient _nbpApiClient;
        private readonly IGoldPriceRepository _goldRepo;

        public Handler(INbpApiClient nbpApiClient, IGoldPriceRepository goldRepo)
        {
            _nbpApiClient = nbpApiClient;
            _goldRepo = goldRepo;
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
    }
}

public class GoldPriceResult : AppResult
{
    public decimal StartDatePrice { get; init; }
    public decimal EndDatePrice { get; init; }
    public decimal AveragePrice { get; init; }
}