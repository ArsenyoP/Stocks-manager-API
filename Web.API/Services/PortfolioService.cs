using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Models;

namespace Web.API.Services
{
    public class PortfolioService : IPorfolioService
    {
        private readonly IPorrfolioRepository _porrfolioRepo;
        private readonly IStockRepository _stockRepo;

        public PortfolioService(IPorrfolioRepository porrfolioRepo, IStockRepository stockRepo)
        {
            _porrfolioRepo = porrfolioRepo;
            _stockRepo = stockRepo;
        }

        public async Task<Portfolio> AddToPortfolio(string symbol, string userID, CancellationToken ct)
        {
            var symbolUpper = symbol.ToUpper();
            var stockID = await _stockRepo.GetIdBySymbolAsync(symbolUpper);

            if (stockID == 0) throw new KeyNotFoundException($"Can't find stock with symbol: {symbol}");

            var isExists = await _porrfolioRepo.CheckIfExistsAsync(userID, stockID, ct);
            if (isExists) throw new ArgumentException("Can't add stock twice");

            var portfolioModel = new Portfolio
            {
                StockId = stockID,
                AppUserId = userID
            };
            await _porrfolioRepo.CreatePortfolioAsync(portfolioModel, ct);
            return portfolioModel;
        }

        public async Task DeletePortfolioAsync(string symbol, string userID, CancellationToken ct = default)
        {
            var symbolUpper = symbol.ToUpper().Trim();

            var portfolioModel = await _porrfolioRepo.GetByIdAndSymbol(userID, symbolUpper, ct);

            if (portfolioModel == null)
            {
                throw new KeyNotFoundException($"Can't find stock with symbol: {symbol}");
            }

            await _porrfolioRepo.DeletePortfolioAsync(portfolioModel, ct);
        }
    }
}
