using CoinPulse.Core;

namespace CoinPulse.Services;

public interface ICoinService
{
    Task<List<Coin>> GetTopCoinsAsync(string currency = "usd", CancellationToken token = default);

    Task UpdateUserCoinAsync(Coin coin);
}