using System.Text.Json.Serialization;

namespace Web.API.Dtos.FMP
{
    public class FMPProfileDto
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("companyName")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("marketCap")]
        public decimal MarketCap { get; set; }

        [JsonPropertyName("sector")]
        public string Industry { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("lastDividend")]
        public decimal Dividend { get; set; }
    }

    public class FMPRenewDto
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("lastDividend")]
        public decimal Dividend { get; set; }
    }
}



