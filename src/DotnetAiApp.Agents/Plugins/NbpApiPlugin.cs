using System.ComponentModel;
using System.Text.Json;
using FluentValidation;
using Microsoft.SemanticKernel;
using DotnetAiApp.NbpApiClient.NbpApiClient;

namespace DotnetAiApp.Agents.Plugins;

public class NbpApiPlugin
{
    private readonly INbpApiClient _nbpApiClient;
    private readonly IValidator<IGetGoldPricesRequest> _validator;

    public NbpApiPlugin(INbpApiClient nbpApiClient,
        GoldPricesRequestValidator validator)
    {
        _nbpApiClient = nbpApiClient;
        _validator = validator;
    }

    [KernelFunction, Description("Gets an array of gold prices between startDate and endDate." +
                                 "All dates are in yyyy-mm-dd format." +
                                 "Returns an array of prices of 1g of gold in polish złoty in json format")]
    public async Task<string> GetGoldPrices(string startDate, string endDate)
    {
        var request = new GoldPricesRequest(DateTime.Parse(startDate), DateTime.Parse(endDate));

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return $"Invalid request: {validationResult}";

        var goldPrices = await _nbpApiClient.GetGoldPricesAsync(request);

        return JsonSerializer.Serialize(goldPrices.Select(p => new
        {
            date = p.Date,
            price = p.Price
        }));
    }
}

public record GoldPricesRequest(DateTime? StartDate, DateTime? EndDate) : IGetGoldPricesRequest;
