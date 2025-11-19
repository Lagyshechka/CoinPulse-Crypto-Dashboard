using System.Text.Json.Serialization;

namespace CoinPulse.Core;

public class Coin
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("image")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("current_price")]
    public decimal? CurrentPrice { get; set; }

    [JsonPropertyName("market_cap")]
    public long? MarketCap { get; set; }
    
    [JsonPropertyName("total_volume")]
    public long? TotalVolume { get; set; }
    
    [JsonPropertyName("high_24h")]
    public decimal? High24H { get; set; }
    
    [JsonPropertyName("low_24h")]
    public decimal? Low24H { get; set; }
    
    [JsonPropertyName("price_change_percentage_24h")]
    public double? PriceChangePercentage24H { get; set; }
    
    [JsonPropertyName("sparkline_in_7d")]
    public SparklineData? SparklineIn7D { get; set; }
}

public class SparklineData
{
    [JsonPropertyName("price")]
    public List<double>? Price { get; set; }
}