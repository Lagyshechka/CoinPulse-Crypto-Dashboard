using System.Linq;
using System.Collections.Generic;
using CoinPulse.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace CoinPulse.UI;

public class CoinViewModel : ObservableObject
{
    public Coin Model { get; }
    
    public string Name => Model.Name;
    public string Symbol => Model.Symbol.ToUpper();
    public string ImageUrl => Model.ImageUrl;
    public decimal CurrentPrice => Model.CurrentPrice ?? 0;
    public double PriceChangePercentage24H => Model.PriceChangePercentage24H ?? 0;
    
    
    public decimal High24h => Model.High24h ?? 0;
    public decimal Low24h => Model.Low24h ?? 0;
    public long MarketCap => Model.MarketCap ?? 0;
    public long TotalVolume => Model.TotalVolume ?? 0;
    

    public string PriceChangeColor => PriceChangePercentage24H >= 0 ? "#4CAF50" : "#F44336";
    
    public string PriceChangeBackGround => PriceChangePercentage24H >= 0 ? "#1A4CAF50" : "#1AF44336";

    public ISeries[] ChartSeries { get; }
    public Axis[] XAxes { get; }
    public Axis[] YAxes { get; }
    
    public CoinViewModel(Coin coin)
    {
        Model = coin;

        var skColor = PriceChangePercentage24H >= 0
            ? SKColor.Parse("#4CAF50")
            : SKColor.Parse("#F44336");
        
        var sparkline = coin.SparklineIn7D;
        List<double> values;

        if (sparkline != null && sparkline.Price != null)
        {
            values = sparkline.Price;
        }
        else
        {
            values = new List<double>();
        }

        ChartSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = values,
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(skColor) { StrokeThickness = 3 },
                LineSmoothness = 0.5
            }
        };
        
        XAxes = new[]
        {
            new Axis { IsVisible = false } 
        };
        
        double? minVal = values.Count > 0 ? values.Min() : null;
        
        YAxes = new[]
        {
            new Axis 
            { 
                IsVisible = false,     
                MinLimit = minVal      
            }
        };
    }
}