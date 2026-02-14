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

        public async Task<Stock?> GetFullStock(string symbol)
        {
            var key = _config["FMP:ApiKey"];

            var stock = await _httpClient.GetFromJsonAsync<FMPProfileDto[]>($"https://financialmodelingprep.com/stable/profile?symbol={symbol}&apikey={key}");

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
                LastUpdate = DateTime.Now
            };
        }

        public async Task<FMPRenewDto[]> GetUpdatedStock(string symbol)
        {
            var key = _config["FMP:ApiKey"];

            var updatedPrice = await _httpClient.GetFromJsonAsync<FMPRenewDto[]>($"https://financialmodelingprep.com/stable/profile?symbol={symbol}&apikey={key}");

            return updatedPrice;
        }

        //public async Task<FMPPriceDto[]> Test(string symbol)
        //{
        //    var key = "lAVIKI1ByTv3kh8bTFtR2tuHM3JHIAEY";

        //    var updatedPrice = await _httpClient.GetFromJsonAsync<FMPRenewDto[]>($"https://financialmodelingprep.com/stable/income-statement?symbol={symbol}&apikey={key}");

        //    return updatedPrice;
        //}
    }
}
