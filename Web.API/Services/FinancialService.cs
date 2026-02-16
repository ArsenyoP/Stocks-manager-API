using Web.API.Dtos.FMP;
using Web.API.Interfaces.IServices;
using Web.API.Models;

namespace Web.API.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public FinancialService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<Stock?> GetFullStock(string symbol, CancellationToken ct)
        {
            var key = _config["FMP:ApiKey"];

            var stock = await _httpClient.GetFromJsonAsync<FMPProfileDto[]>($"https://financialmodelingprep.com/stable/profile?symbol={symbol}&apikey={key}", ct);

            var profile = stock?.FirstOrDefault();

            if (profile == null)
            {
                return null;
            }

            return new Stock
            {
                CompanyName = profile.Name,
                Symbol = profile.Symbol,
                Industy = profile.Industry,
                MarketCap = (long)Math.Round(profile.MarketCap, 0, MidpointRounding.AwayFromZero),
                Purchase = profile.Price,
                LastDiv = profile.Dividend,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now,
                Comments = new List<Comment>()
            };
        }

        public async Task<FMPRefreshDto?> GetRefreshedStockDto(string symbol, CancellationToken ct)
        {
            var key = _config["FMP:ApiKey"];

            var updatedPrice = await _httpClient.GetFromJsonAsync<FMPRefreshDto[]>($"https://financialmodelingprep.com/stable/profile?symbol={symbol}&apikey={key}", ct);

            if (updatedPrice == null)
            {
                return null;
            }

            var result = updatedPrice.FirstOrDefault();
            return result;
        }
    }
}
