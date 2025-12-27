using System.ComponentModel.DataAnnotations.Schema;

namespace Web.API.Dtos.Stock
{
    //шаблон для заповнення даних про створення об'єкту
    public class CreateStockRequestDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal Purchase { get; set; }
        public decimal LastDiv { get; set; }
        public string Industy { get; set; } = string.Empty;
        public long MarketCap { get; set; }
    }

    public class CreateLightStockRequestDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
    }
}
