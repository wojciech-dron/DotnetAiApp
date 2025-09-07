using DotnetAiApp.Core.Utils;
using DotnetAiApp.Tests.Mocks;
using FluentAssertions;
using DotnetAiApp.Db.Entities;
using DotnetAiApp.NbpApiClient.NbpApiClient;
using DotnetAiApp.Web.Logic.Commands;
using NSubstitute;

namespace DotnetAiApp.Tests.Logic.Commands;

public class GetAndSaveGoldPricesTests
{
    private readonly INbpApiClient _nbpApiClient;
    private readonly IFileProvider _fileProvider;
    private readonly TimeProviderMock _timeProvider;
    private readonly DotentAiAppContextMock _dbContext;

    private readonly GetAndSaveGoldPrices.Handler _handler;

    public GetAndSaveGoldPricesTests()
    {
        _nbpApiClient = Substitute.For<INbpApiClient>();
        _dbContext = DotentAiAppContextMock.Create();
        _fileProvider = Substitute.For<IFileProvider>();
        _timeProvider = new TimeProviderMock();

        _handler = new GetAndSaveGoldPrices.Handler(_nbpApiClient, _fileProvider, _timeProvider, _dbContext);
    }

    [Fact]
    public async Task Handle_NoGoldPricesFound_ReturnsErrorMessage()
    {
        // Arrange
        var command = new GetAndSaveGoldPrices.Command();
        _nbpApiClient.GetGoldPricesAsync(command, Arg.Any<CancellationToken>()).Returns([]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ErrorMessage.Should().Be("No gold prices found");
    }

    [Fact]
    public async Task Handle_GoldPricesFound_SavesToDbAndFile()
    {
        // Arrange
        var command = new GetAndSaveGoldPrices.Command();
        var goldPrices = new[]
        {
            new NpbPriceDto { Date = new DateOnly(2023, 10, 1), Price = 100m },
            new NpbPriceDto { Date = new DateOnly(2023, 10, 2), Price = 150m }
        };
        _nbpApiClient.GetGoldPricesAsync(command, Arg.Any<CancellationToken>()).Returns(goldPrices);
        var currentTime = new DateTime(2023, 10, 3, 12, 0, 0);
        _timeProvider.CurrentTime = currentTime;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.GoldPrices.Should().BeEquivalentTo([
            new GoldPrice { Date = goldPrices[0].Date, Price = 100 },
            new GoldPrice { Date = goldPrices[1].Date, Price = 150 }
        ]);

        await _fileProvider.Received().WriteTextAsync(
            $"gold_prices_request_{currentTime:yyyy-MM-dd_HH-mm-ss}.json",
            """[{"date":"2023-10-01","price":100},{"date":"2023-10-02","price":150}]""",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_GoldPricesFound_MergesWithExistingDbData()
    {
        // Arrange
        _dbContext.GoldPrices.AddRange(
            new GoldPrice { Date = new DateOnly(2023, 10, 1), Price = 100 },
            new GoldPrice { Date = new DateOnly(2023, 10, 2), Price = 150 }
        );
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var command = new GetAndSaveGoldPrices.Command();
        var retrieved = new[]
        {
            new NpbPriceDto { Date = new DateOnly(2023, 10, 2), Price = 200m },
            new NpbPriceDto { Date = new DateOnly(2023, 10, 3), Price = 250m }
        };
        _nbpApiClient.GetGoldPricesAsync(command, Arg.Any<CancellationToken>()).Returns(retrieved);
        _timeProvider.CurrentTime = new DateTime(2023, 10, 3, 12, 0, 0);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.GoldPrices.Should().BeEquivalentTo([
            new GoldPrice { Date = new DateOnly(2023, 10, 1), Price = 100 },
            new GoldPrice { Date = new DateOnly(2023, 10, 2), Price = 200 },
            new GoldPrice { Date = new DateOnly(2023, 10, 3), Price = 250 },
        ]);
    }

    [Fact]
    public async Task Handle_GoldPricesFound_ReturnsCorrectResult()
    {
        // Arrange
        var command = new GetAndSaveGoldPrices.Command();
        var goldPrices = new[]
        {
            new NpbPriceDto { Date = new DateOnly(2023, 10, 1), Price = 100m },
            new NpbPriceDto { Date = new DateOnly(2023, 10, 2), Price = 150m },
        };
        _nbpApiClient.GetGoldPricesAsync(command, Arg.Any<CancellationToken>()).Returns(goldPrices);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.StartDatePrice.Should().Be(100m);
        result.EndDatePrice.Should().Be(150m);
        result.AveragePrice.Should().Be(125m);
    }
}