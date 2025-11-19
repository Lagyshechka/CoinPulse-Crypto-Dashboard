using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoinPulse.Services;
using CoinPulse.UI.Services;

namespace CoinPulse.UI;

public partial class MainViewModel : ObservableObject
{
    private readonly ICoinService _coinService;
    private readonly IDispatchedService _dispatchedService;
    private readonly INavigationService _navigationService;
    
    private List<CoinViewModel> _allCoins = new(); 
    
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "Ready to update";
    [ObservableProperty] private string _searchText = string.Empty;
    
    [ObservableProperty] private CoinViewModel? _selectedCoin;

    public ObservableCollection<CoinViewModel> Coins { get; } = new();

    public MainViewModel(ICoinService coinService, INavigationService navigationService, IDispatchedService dispatchedService)
    {
        _coinService = coinService;
        _navigationService = navigationService;
        _dispatchedService = dispatchedService;
        if (Application.Current?.Dispatcher != null)
        {
            Task.Run(LoadData);
        }
    }

    [RelayCommand]
    public void OpenDetails(CoinViewModel? coin)
    {
        var target = coin ?? SelectedCoin;
        
        if (target == null) return;
        _navigationService.OpenCoinDetails(target);
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterCoins();
    }

    private void FilterCoins()
    {
        IEnumerable<CoinViewModel> filtered;

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = _allCoins;
        }
        else
        {
            filtered = _allCoins.Where(c => 
                c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                c.Symbol.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        _dispatchedService.Invoke(() =>
        {
            Coins.Clear();
            foreach (var coin in filtered)
            {
                Coins.Add(coin);
            }
        });
    }

    [RelayCommand]
    public async Task LoadData()
    {
        if (IsLoading) return;
        IsLoading = true;
        StatusMessage = "Fetching data...";

        try
        {
            var result = await _coinService.GetTopCoinsAsync();

            _dispatchedService.Invoke(() =>
            {
                _allCoins = result.Select(c => new CoinViewModel(c)).ToList();
                FilterCoins();
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