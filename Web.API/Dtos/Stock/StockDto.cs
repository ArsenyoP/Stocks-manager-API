using System.ComponentModel.DataAnnotations.Schema;

namespace Web.API.Dtos.Stock
{
    public class StockDto
    {
        public int ID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal Purchase { get; set; }
        public decimal LastDiv { get; set; }
        public string Industy { get; set; } = string.Empty;
        public long MarketCap { get; set; }

    }
}
