using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoinPulse.Core;
using CoinPulse.Services;

namespace CoinPulse.UI;

public partial class MainViewModel : ObservableObject
{
    private readonly ICoinService _coinService;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "Ready to update";

    public ObservableCollection<Coin> Coins { get; } = new();

    public MainViewModel(ICoinService coinService)
    {
        _coinService = coinService;
        Task.Run(LoadData);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (IsLoading) return;

        IsLoading = true;
        StatusMessage = "Fetching data...";

        try
        {
            var result = await _coinService.GetTopCoinsAsync();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Coins.Clear();
                foreach (var coin in result)
                {
                    Coins.Add(coin);
                }
            });

            StatusMessage = $"Updated: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}