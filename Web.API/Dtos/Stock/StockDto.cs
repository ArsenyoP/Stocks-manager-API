using System.ComponentModel.DataAnnotations.Schema;
using Web.API.Dtos.Comment;
using Web.API.Models;

namespace Web.API.Dtos.Stock
{
    //шаблон даних для відправлення користувачу
    public class StockDto
    {
        public int ID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal Purchase { get; set; }
        public decimal LastDiv { get; set; }
        public string Industy { get; set; } = string.Empty;
        public long MarketCap { get; set; }
        public List<CommentDto> Comments { get; set; }

    }

    public class StockPortfolioDto
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
