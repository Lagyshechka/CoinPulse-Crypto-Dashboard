using System.Windows;

namespace CoinPulse.UI;

public partial class PortfolioWindow : Window
{
    public PortfolioWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}