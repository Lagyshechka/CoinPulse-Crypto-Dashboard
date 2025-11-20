using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoinPulse.Services;
using CoinPulse.UI.Services;

namespace CoinPulse.UI;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly ICoinService _coinService;
    private readonly IDispatchedService _dispatchedService;
    private readonly INavigationService _navigationService;
    
    private readonly System.Timers.Timer _refreshTimer;
    private const int RefreshIntervalMs = 60000;
    
    private CancellationTokenSource? _loadDataCts;
    private CancellationTokenSource? _currencyDebounceCts;
    
    private List<CoinViewModel> _allCoins = new(); 
    
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "Ready to update";
    [ObservableProperty] private string _searchText = string.Empty;
    
    [ObservableProperty] private CoinViewModel? _selectedCoin;
    [ObservableProperty] private bool _isAutoRefreshEnabled;

    [ObservableProperty] private decimal _totalPortfolioValue;

    [ObservableProperty] private string _selectedCurrency = "usd";
    public ObservableCollection<string> Currencies { get; } = new() { "usd", "eur", "rub", "uah", "jpy", "gbp", "btc", "eth" };
    
    public string CurrentCurrencySymbol => SelectedCurrency switch
    {
        "usd" => "$",
        "eur" => "€",
        "rub" => "₽",
        "uah" => "₴",
        "jpy" => "¥",
        "gbp" => "£",
        "btc" => "₿",
        "eth" => "Ξ",
        _ => SelectedCurrency.ToUpper()
    };

    public ObservableCollection<CoinViewModel> Coins { get; } = new();

    public MainViewModel(ICoinService coinService, INavigationService navigationService, IDispatchedService dispatchedService)
    {
        _coinService = coinService;
        _navigationService = navigationService;
        _dispatchedService = dispatchedService;
        
        _refreshTimer = new System.Timers.Timer(RefreshIntervalMs);
        _refreshTimer.Elapsed += OnTimerElapsed;

        Task.Run(LoadData);
    }

    partial void OnSelectedCurrencyChanged(string value)
    {
        OnPropertyChanged(nameof(CurrentCurrencySymbol));
        
        _currencyDebounceCts?.Cancel();
        _currencyDebounceCts = new CancellationTokenSource();
        var token = _currencyDebounceCts.Token;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(1000, token);

                if (token.IsCancellationRequested) return;

                await LoadData();
            }
            catch (TaskCanceledException)
            {

            }
        });
    }

    private void OnCoinViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CoinViewModel.Amount))
        {
            RecalculateTotalValue();

            if (sender is CoinViewModel vm)
            {
                Task.Run(() => _coinService.UpdateUserCoinAsync(vm.Model));
            }
        }
    }

    private void RecalculateTotalValue()
    {
        TotalPortfolioValue = _allCoins.Sum(c => c.HeldValue);
    }

    partial void OnIsAutoRefreshEnabledChanged(bool value)
    {
        if (value)
        {
            _refreshTimer.Start();
            StatusMessage = "Auto-refresh enabled";
        }
        else
        {
            _refreshTimer.Stop();
            StatusMessage = "Auto-refresh disabled";
        }
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        Task.Run(LoadData);
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
        _loadDataCts?.Cancel();
        _loadDataCts = new CancellationTokenSource();
        var token = _loadDataCts.Token;
        
        IsLoading = true;
        StatusMessage = "Fetching data...";

        try
        {
            var result = await _coinService.GetTopCoinsAsync(SelectedCurrency, token);
            var symbol = CurrentCurrencySymbol;

            if (token.IsCancellationRequested) return;

            _dispatchedService.Invoke(() =>
            {
                foreach (var coin in _allCoins)
                    coin.PropertyChanged -= OnCoinViewModelPropertyChanged;

                _allCoins = result.Select(c => new CoinViewModel(c, symbol)).ToList();

                foreach (var coin in _allCoins)
                    coin.PropertyChanged += OnCoinViewModelPropertyChanged;

                FilterCoins();
                RecalculateTotalValue();
            });

            StatusMessage = $"Updated: {DateTime.Now:HH:mm:ss}";
        }
        catch (TaskCanceledException)
        {
            
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                IsLoading = false;
            }
        }
    }

    public void Dispose()
    {
        _refreshTimer?.Dispose();
        _loadDataCts?.Cancel();
        _currencyDebounceCts?.Cancel();
        foreach(var coin in _allCoins)
            coin.PropertyChanged -= OnCoinViewModelPropertyChanged;
    }
}