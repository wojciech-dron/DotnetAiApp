using MediatR;
using NbpApp.Db;
using NbpApp.Utils.Extensions;
using NbpApp.Utils.Pagination;
using NbpApp.Web.Dtos;

namespace NbpApp.Web.Logic.Queries;

public class GetSavedPrices
{
    public class Query : Pager, IRequest<PagedList<SavedPriceDto>>
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class Handler : IRequestHandler<Query, PagedList<SavedPriceDto>>
    {
        private readonly NbpAppContext _context;

        public Handler(NbpAppContext context)
        {
            _context = context;
        }

        public async Task<PagedList<SavedPriceDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _context.GoldPrices
                .WhereIf(request.StartDate.HasValue, gp => gp.Date >= request.StartDate)
                .WhereIf(request.EndDate.HasValue, gp => gp.Date <= request.EndDate)
                .Order(request)
                .Select(gp => new SavedPriceDto(gp.Date, (decimal)gp.Price))
                .ToPagedResponseAsync(request, cancellationToken);

            return result;
        }
    }
}