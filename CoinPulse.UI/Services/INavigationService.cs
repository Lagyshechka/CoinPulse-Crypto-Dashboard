namespace CoinPulse.UI.Services;

public interface INavigationService
{
    void OpenCoinDetails(CoinViewModel coin);
    
    void OpenPortfolio(MainViewModel mainViewModel);
}