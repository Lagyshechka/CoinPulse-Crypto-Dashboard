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
    private readonly INavigationService _navigationService;

    private List<CoinViewModel> _allCoins = new(); 
    
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "Ready to update";
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private CoinViewModel? _selectedCoin;

    public ObservableCollection<CoinViewModel> Coins { get; } = new();

    public MainViewModel(ICoinService coinService, INavigationService navigationService)
    {
        _coinService = coinService;
        _navigationService = navigationService;
        Task.Run(LoadData);
    }

    [RelayCommand]
    private void OpenDetails(CoinViewModel? coin)
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
        if (_allCoins is null || _allCoins.Count == 0) return;

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

        Application.Current.Dispatcher.Invoke(() =>
        {
            Coins.Clear();
            foreach (var coin in filtered)
            {
                Coins.Add(coin);
            }
        });
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

            Application.Current.Dispatcher.Invoke(() =>
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