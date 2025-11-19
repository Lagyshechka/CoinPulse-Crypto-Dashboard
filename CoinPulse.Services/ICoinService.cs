using CoinPulse.Core;

namespace CoinPulse.Services;

public interface ICoinService
{
    Task<List<Coin>> GetTopCoinsAsync(CancellationToken token = default);
}