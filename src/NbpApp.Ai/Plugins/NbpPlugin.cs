using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using NbpApp.Db.Entities;
using NbpApp.Db.Repos;
using NbpApp.NbpApiClient;
using NbpApp.NbpApiClient.Contracts;

namespace NbpApp.Ai.Plugins;

public class NbpPlugin
{
    private readonly INbpApiClient _nbpApiClient;
    private readonly IGoldPriceRepository _goldRepo;

    public NbpPlugin(INbpApiClient nbpApiClient, IGoldPriceRepository goldRepo)
    {
        _nbpApiClient = nbpApiClient;
        _goldRepo = goldRepo;
    }

    [KernelFunction, Description("Gets an array of gold prices between startDate and endDate." +
                                 "Dates are in yyyy-mm-dd format.")]
    [return: Description("An array of prices (in PLN) in json format")]
    public async Task<string> GetGoldPrices(DateOnly startDate, DateOnly endDate)
    {
        var prices = await _nbpApiClient.GetGoldPricesAsync(startDate, endDate);

        return JsonSerializer.Serialize(prices);
    }

    [KernelFunction, Description("Saves provided gold prices to database")]
    [return: Description("Result message")]
    public async Task<string> SavePricesToDb(GoldPrice[] prices)
    {
        await _goldRepo.AddOrUpdatePrices(prices);

        return "Saved prices in db.";
    }

}