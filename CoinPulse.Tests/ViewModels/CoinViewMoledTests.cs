using CoinPulse.Core;
using CoinPulse.UI;
using Xunit;

namespace CoinPulse.Tests.ViewModels;

public class CoinViewModelTests
{
    [Fact]
    public void PriceChangeColor_ShouldBeGreen_WhenPositive()
    {
        var coin = new Coin { PriceChangePercentage24H = 5.5 };
        var vm = new CoinViewModel(coin);

        Assert.Equal("#4CAF50", vm.PriceChangeColor);
    }

    [Fact]
    public void PriceChangeColor_ShouldBeRed_WhenNegative()
    {
        var coin = new Coin { PriceChangePercentage24H = -1.2 };
        var vm = new CoinViewModel(coin);

        Assert.Equal("#F44336", vm.PriceChangeColor);
    }

    [Fact]
    public void Symbol_ShouldBeUpperCase()
    {
        var coin = new Coin { Symbol = "btc" };
        var vm = new CoinViewModel(coin);

        Assert.Equal("BTC", vm.Symbol);
    }
}