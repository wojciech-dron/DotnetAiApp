using DotnetAiApp.Core.Extensions;
using DotnetAiApp.Core.Pagination;
using DotnetAiApp.Web.Dtos;
using Microsoft.EntityFrameworkCore;
using DotnetAiApp.Db;
using Mediator;
using EFCoreSecondLevelCacheInterceptor;

namespace DotnetAiApp.Web.Logic.Queries;

public class GetSavedPrices
{
    public class Query : Pager, IRequest<PagedList<SavedPriceDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
    }

    public class Handler : IRequestHandler<Query, PagedList<SavedPriceDto>>
    {
        private readonly DotentAiAppContext _context;

        public Handler(DotentAiAppContext context)
        {
            _context = context;
        }

        public async ValueTask<PagedList<SavedPriceDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _context.GoldPrices
                .AsNoTracking()
                .WhereIf(request.StartDate.HasValue, gp => gp.Date >= request.StartDate.Value.Date)
                .WhereIf(request.EndDate.HasValue, gp => gp.Date <= request.EndDate)
                .WhereIf(request.MinPrice.HasValue, gp => gp.Price >= request.MinPrice)
                .WhereIf(request.MaxPrice.HasValue, gp => gp.Price <= request.MaxPrice)
                .Order(request)
                .Select(gp => new SavedPriceDto(gp.Date, (decimal)gp.Price))
                .Cacheable(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(5))
                .ToPagedResponseAsync(request, cancellationToken);

            return result;
        }
    }
}