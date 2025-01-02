using System.ComponentModel;
using System.Text.Json;
using FluentValidation;
using Microsoft.SemanticKernel;
using NbpApp.NbpApiClient;
using NbpApp.NbpApiClient.Validators;

namespace NbpApp.Ai.Plugins;

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
    public async Task<string> GetGoldPrices(DateOnly startDate, DateOnly endDate)
    {
        var request = new GoldPricesRequest(startDate, endDate);

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

public record GoldPricesRequest(DateOnly? StartDate, DateOnly? EndDate) : IGetGoldPricesRequest;
