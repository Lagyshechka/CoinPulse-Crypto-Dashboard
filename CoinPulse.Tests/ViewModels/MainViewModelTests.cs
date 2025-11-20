using CoinPulse.Core;
using CoinPulse.Services;
using CoinPulse.UI;
using CoinPulse.UI.Services;
using Moq;
using Xunit;

namespace CoinPulse.Tests.ViewModels;

public class MainViewModelTests
{
    private readonly Mock<ICoinService> _mockCoinService;
    private readonly Mock<INavigationService> _mockNavService;
    private readonly Mock<IDispatchedService> _mockDispatchedService;
    private readonly MainViewModel _viewModel;

    public MainViewModelTests()
    {
        _mockCoinService = new Mock<ICoinService>();
        _mockNavService = new Mock<INavigationService>();
        _mockDispatchedService = new Mock<IDispatchedService>();
        
        _mockDispatchedService.Setup(d => d.Invoke(It.IsAny<Action>()))
            .Callback<Action>(action => action());

        _viewModel = new MainViewModel(_mockCoinService.Object, _mockNavService.Object, _mockDispatchedService.Object);
    }

    [Fact]
    public async Task LoadData_ShouldPopulateCoins_WhenServiceReturnsData()
    {
        var fakeCoins = new List<Coin>
        {
            new() { Id = "bitcoin", Name = "Bitcoin", Symbol = "btc", CurrentPrice = 50000 },
            new() { Id = "ethereum", Name = "Ethereum", Symbol = "eth", CurrentPrice = 3000 }
        };

        _mockCoinService.Setup(s => s.GetTopCoinsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeCoins);

        await _viewModel.LoadData();

        Assert.DoesNotContain("Error", _viewModel.StatusMessage);
        
        Assert.Equal(2, _viewModel.Coins.Count);
        Assert.Contains(_viewModel.Coins, c => c.Name == "Bitcoin");
        Assert.Contains(_viewModel.Coins, c => c.Name == "Ethereum");
    }

    [Fact]
    public async Task SearchText_ShouldFilterCoins_ByName()
    {
        var fakeCoins = new List<Coin>
        {
            new() { Name = "Bitcoin", Symbol = "btc" },
            new() { Name = "Dogecoin", Symbol = "doge" },
            new() { Name = "Ethereum", Symbol = "eth" }
        };

        _mockCoinService.Setup(s => s.GetTopCoinsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeCoins);

        await _viewModel.LoadData();
        Assert.DoesNotContain("Error", _viewModel.StatusMessage);

        _viewModel.SearchText = "Bit";

        Assert.Single(_viewModel.Coins);
        Assert.Equal("Bitcoin", _viewModel.Coins.First().Name);
    }

    [Fact]
    public async Task SearchText_ShouldFilterCoins_BySymbol()
    {
        var fakeCoins = new List<Coin>
        {
            new() { Name = "Bitcoin", Symbol = "btc" },
            new() { Name = "Ethereum", Symbol = "eth" }
        };

        _mockCoinService.Setup(s => s.GetTopCoinsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeCoins);

        await _viewModel.LoadData();
        Assert.DoesNotContain("Error", _viewModel.StatusMessage);

        _viewModel.SearchText = "ETH"; 

        Assert.Single(_viewModel.Coins);
        Assert.Equal("Ethereum", _viewModel.Coins.First().Name);
    }
    
    [Fact]
    public void OpenDetails_ShouldCallNavigationService()
    {
        var coinModel = new Coin { Id = "btc", Name = "Bitcoin" };
        var coinVm = new CoinViewModel(coinModel, "$");

        _viewModel.OpenDetails(coinVm);

        _mockNavService.Verify(n => n.OpenCoinDetails(coinVm), Times.Once);
    }
}