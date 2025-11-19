using System.Net.Http.Json;
using CoinPulse.Core;
using CoinPulse.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace CoinPulse.Services;

public class CoinGeckoService : ICoinService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "Https://api.coingecko.com/api/v3";
    private readonly AppDbContext _dbContext;

    public CoinGeckoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _dbContext = new AppDbContext();
        _dbContext.Database.EnsureCreated();
    }

    public async Task<List<Coin>> GetTopCoinsAsync(CancellationToken token = default)
    {
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "CoinPulseApp/1.0");
        var url = $"{BaseUrl}/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=10&page=1&sparkline=true&price_change_percentage=24h";
        try 
        {
            var response = await _httpClient.GetFromJsonAsync<List<Coin>>(url, token);
            
            if (response != null && response.Count > 0)
            {
                await UpdateLocalCacheAsync(response, token);
                return await _dbContext.Coins.AsNoTracking().ToListAsync(token);
            }
        }
        catch (Exception)
        {
            // ignoring error and reading cache 
        }
        
        return await _dbContext.Coins.AsNoTracking().ToListAsync(token);
    }

    public async Task UpdateUserCoinAsync(Coin coin)
    {
        var existing = await _dbContext.Coins.FirstOrDefaultAsync(c => c.Id == coin.Id);
        if (existing != null)
        {
            existing.Amount = coin.Amount;
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task UpdateLocalCacheAsync(List<Coin> newCoins, CancellationToken token)
    {
        try
        {
            foreach (var coin in newCoins)
            {
                var existingCoin = await _dbContext.Coins.FirstOrDefaultAsync(c => c.Id == coin.Id, token);

                if (existingCoin != null)
                {
                    existingCoin.CurrentPrice = coin.CurrentPrice;
                    existingCoin.PriceChangePercentage24H = coin.PriceChangePercentage24H;
                    existingCoin.MarketCap = coin.MarketCap;
                    existingCoin.SparklineIn7D = coin.SparklineIn7D;
                    existingCoin.ImageUrl = coin.ImageUrl;
                    existingCoin.Name = coin.Name;
                    existingCoin.Symbol = coin.Symbol;
                    existingCoin.High24H = coin.High24H;
                    existingCoin.Low24H = coin.Low24H;
                    existingCoin.TotalVolume = coin.TotalVolume;
                }
                else
                {
                    await _dbContext.Coins.AddAsync(coin, token);
                }
            }

            await _dbContext.SaveChangesAsync(token);
        }
        catch
        {
            // caching error shouldn't break application :)
        }
    }
}