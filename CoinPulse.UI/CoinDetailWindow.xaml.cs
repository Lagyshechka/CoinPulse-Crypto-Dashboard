using System.Windows;

namespace CoinPulse.UI
{
    public partial class CoinDetailWindow : Window
    {
        public CoinDetailWindow(CoinViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}