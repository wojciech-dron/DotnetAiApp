using FluentAssertions;
using NbpApp.Db;
using NbpApp.Db.Entities;
using NbpApp.Tests.Mocks;
using NbpApp.Web.Logic.Queries;

namespace NbpApp.Tests.Logic.Queries;

public class GetSavedPricesHandlerTests
{
    private readonly NbpAppContext _context;
    private readonly GetSavedPrices.Handler _handler;

    public GetSavedPricesHandlerTests()
    {
        _context = NbpAppContextMock.Create();
        _handler = new GetSavedPrices.Handler(_context);
    }

    [Fact]
    public async Task NoGoldPricesFound_ReturnsNoPrices()
    {
        // Arrange
        var query = new GetSavedPrices.Query();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenDateRange_ReturnsFilteredResults()
    {
        // Arrange
        var startDate = new DateOnly(2023, 1, 1);
        var endDate = new DateOnly(2023, 1, 31);
        var query = new GetSavedPrices.Query { StartDate = startDate, EndDate = endDate };
        var goldPrices = new List<GoldPrice>
        {
            new() { Date = new DateOnly(2023, 1, 15), Price = 100 },
            new() { Date = new DateOnly(2023, 2, 15), Price = 150 }
        };
        _context.GoldPrices.AddRange(goldPrices);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().HaveCount(1);
        result.Data.First().Date.Should().Be(new DateOnly(2023, 1, 15));
    }

    [Fact]
    public async Task GivenPriceRange_ReturnsFilteredResults()
    {
        // Arrange
        var minPrice = 90.0;
        var maxPrice = 110.0;
        var query = new GetSavedPrices.Query { MinPrice = minPrice, MaxPrice = maxPrice };
        var goldPrices = new List<GoldPrice>
        {
            new() { Date = new DateOnly(2023, 1, 15), Price = 100 },
            new() { Date = new DateOnly(2023, 1, 16), Price = 80 }
        };
        _context.GoldPrices.AddRange(goldPrices);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().HaveCount(1);
        result.Data.First().Price.Should().Be(100);
    }

    [Fact]
    public async Task GivenAllFilters_ReturnsFilteredResults()
    {
        // Arrange
        var startDate = new DateOnly(2023, 1, 1);
        var endDate = new DateOnly(2023, 1, 31);
        var minPrice = 90.0;
        var maxPrice = 110.0;
        var query = new GetSavedPrices.Query
        {
            StartDate = startDate,
            EndDate = endDate,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };
        var goldPrices = new List<GoldPrice>
        {
            new() { Date = new DateOnly(2023, 1, 15), Price = 100 },
            new() { Date = new DateOnly(2023, 1, 16), Price = 80 },
            new() { Date = new DateOnly(2024, 1, 15), Price = 100 }
        };
        _context.GoldPrices.AddRange(goldPrices);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().HaveCount(1);
        result.Data.First().Date.Should().Be(new DateOnly(2023, 1, 15));
        result.Data.First().Price.Should().Be(100);
    }
}