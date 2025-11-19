using System.Windows;
using System.Windows.Input;

namespace CoinPulse.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.OpenDetailsCommand.CanExecute(vm.SelectedCoin))
                {
                    vm.OpenDetailsCommand.Execute(vm.SelectedCoin);
                }
            }
        }
    }
}    