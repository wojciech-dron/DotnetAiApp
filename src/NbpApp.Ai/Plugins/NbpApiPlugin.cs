using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using NbpApp.NbpApiClient;

namespace NbpApp.Ai.Plugins;

public class NbpApiPlugin
{
    private readonly INbpApiClient _nbpApiClient;

    public NbpApiPlugin(INbpApiClient nbpApiClient)
    {
        _nbpApiClient = nbpApiClient;
    }

    [KernelFunction, Description("Gets an array of gold prices between startDate and endDate." +
                                 "Dates are in yyyy-mm-dd format." +
                                 "Returns an array of prices of 1g of gold in polish złoty in json format")]
    public async Task<string> GetGoldPrices(DateOnly startDate, DateOnly endDate)
    {
        var prices = await _nbpApiClient.GetGoldPricesAsync(startDate, endDate);

        return JsonSerializer.Serialize(prices);
    }
}