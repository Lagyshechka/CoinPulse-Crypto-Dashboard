using System.Windows;

namespace CoinPulse.UI.Services;

public class NavigationService : INavigationService
{
    public void OpenCoinDetails(CoinViewModel coin)
    {
        var detailWindow = new CoinDetailWindow(coin);
        if (Application.Current.MainWindow != null)
        {
            detailWindow.Owner = Application.Current.MainWindow;
        }
        detailWindow.ShowDialog();
    }

    public void OpenPortfolio(MainViewModel mainViewModel)
    {
        var portfolioWindow = new PortfolioWindow(mainViewModel);

        if (Application.Current.MainWindow != null)
        {
            portfolioWindow.Owner = Application.Current.MainWindow;
        }

        portfolioWindow.ShowDialog();
    }
}