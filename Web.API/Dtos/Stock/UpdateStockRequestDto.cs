using System.ComponentModel.DataAnnotations;

namespace Web.API.Dtos.Stock
{
    public class UpdateStockRequestDto
    {
        [Required]
        [MaxLength(15, ErrorMessage = "Company name cannot be over 15 characters")]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 characters")]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000000000)]
        public decimal Purchase { get; set; }

        [Required]
        [Range(0.001, 100)]
        public decimal LastDiv { get; set; }

        [Required]
        [MaxLength(15, ErrorMessage = "Industry cannot be over 15 characters")]
        public string Industy { get; set; } = string.Empty;

        [Range(1, 5000000000)]
        public long MarketCap { get; set; }
    }
}
