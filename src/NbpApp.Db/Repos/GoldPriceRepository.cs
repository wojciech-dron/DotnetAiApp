using EFCore.BulkExtensions;
using NbpApp.Db.Entities;

namespace NbpApp.Db.Repos;

public interface IGoldPriceRepository
{
    public Task AddOrUpdatePrices(IEnumerable<GoldPrice> prices,
        CancellationToken cancellationToken = default);
}

internal class GoldPriceRepository : IGoldPriceRepository
{
    private readonly NbpAppContext _context;

    public GoldPriceRepository(NbpAppContext context)
    {
        _context = context;
    }

    public async Task AddOrUpdatePrices(IEnumerable<GoldPrice> entities,
        CancellationToken cancellationToken = default)
    {
        await _context.BulkInsertOrUpdateAsync(entities, cancellationToken: cancellationToken);
    }
}