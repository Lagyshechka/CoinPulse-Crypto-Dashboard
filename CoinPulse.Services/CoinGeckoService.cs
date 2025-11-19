using System.Net.Http.Json;
using CoinPulse.Core;

namespace CoinPulse.Services;

public class CoinGeckoService : ICoinService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "Https://api.coingecko.com/api/v3";

    public CoinGeckoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Coin>> GetTopCoinsAsync(CancellationToken token = default)
    {
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "CoinPulseApp/1.0");

        var url = $"{BaseUrl}/coins/markets?vs_currency = usd&order=market_cap_desc&per_page=10&page=1&sparkline=false";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Coin>>(url, token);
            return response ?? new List<Coin>();
        }
        catch (Exception)
        {
            return new List<Coin>();
        }
    }
}