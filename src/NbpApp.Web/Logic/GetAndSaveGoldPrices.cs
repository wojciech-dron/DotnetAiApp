using MediatR;
using NbpApp.NbpApiClient;
using NbpApp.Web.Common;

namespace NbpApp.Web.Logic;

public class GetAndSaveGoldPrices
{
    public record Command(DateOnly StartDate, DateOnly EndDate) : IRequest<GoldPriceResult>;

    class Handler : IRequestHandler<Command, GoldPriceResult>
    {
        private readonly INbpApiClient _nbpApiClient;

        public Handler(INbpApiClient nbpApiClient)
        {
            _nbpApiClient = nbpApiClient;
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

            var viewModel = new GoldPriceResult
            {
                StartDatePrice = goldPrices.First().Price,
                EndDatePrice = goldPrices.Last().Price,
                AveragePrice = goldPrices.Average(p => p.Price)
            };

            return viewModel;
        }
    }
}

public class GoldPriceResult : AppResult
{
    public decimal StartDatePrice { get; init; }
    public decimal EndDatePrice { get; init; }
    public decimal AveragePrice { get; init; }
}