using System.ComponentModel.DataAnnotations;
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
        [Required]
        [MaxLength(15, ErrorMessage = "Company name cannot be over 15 characters")]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 characters")]
        public string Symbol { get; set; } = string.Empty;
    }
}
