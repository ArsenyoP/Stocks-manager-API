using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Web.API.Dtos.Stock;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Mappers;
using Web.API.Models;

namespace Web.API.Services
{
    public class PortfolioService : IPorfolioService
    {
        private readonly IPorrfolioRepository _porrfolioRepo;
        private readonly IStockRepository _stockRepo;
        private readonly ILogger<PortfolioService> _logger;

        public PortfolioService(IPorrfolioRepository porrfolioRepo, IStockRepository stockRepo, ILogger<PortfolioService> logger)
        {
            _porrfolioRepo = porrfolioRepo;
            _stockRepo = stockRepo;
            _logger = logger;
        }

        public async Task<Portfolio> AddToPortfolio(string symbol, string userID, CancellationToken ct)
        {
            var symbolUpper = symbol.ToUpper();
            var stockID = await _stockRepo.GetIdBySymbolAsync(symbolUpper);

            if (stockID == 0)
            {
                _logger.LogWarning("User with ID: {UserID} tried to add to portfolio" +
                    "non-existent stock with symnol: {Symbol}",
                    userID, symbol);
                throw new KeyNotFoundException($"Can't find stock with symbol: {symbol}");
            }

            var isExists = await _porrfolioRepo.CheckIfExistsAsync(userID, stockID, ct);
            if (isExists)
            {
                _logger.LogWarning("User with ID: {UserID} tried to add to portfolio" +
                    "repetitive stock with id: {StockID}",
                    userID, stockID);
                throw new ArgumentException("Can't add stock twice");
            }

            var portfolioModel = new Portfolio
            {
                StockId = stockID,
                AppUserId = userID
            };
            await _porrfolioRepo.CreatePortfolioAsync(portfolioModel, ct);
            _logger.LogInformation("User with ID: {UserID} added to portfolio" +
                    "stock with id: {StockID}",
                    userID, stockID);

            return portfolioModel;
        }

        public async Task DeletePortfolioAsync(string symbol, string userID, CancellationToken ct = default)
        {
            var symbolUpper = symbol.ToUpper().Trim();

            var portfolioModel = await _porrfolioRepo.GetByIdAndSymbol(userID, symbolUpper, ct);

            if (portfolioModel == null)
            {
                _logger.LogWarning("Delete portfolio item failed: Stock {Symbol} not found in portfolio for user {UserId}",
                    symbolUpper, userID);
                throw new KeyNotFoundException($"Can't find stock with symbol: {symbol}");
            }

            await _porrfolioRepo.DeletePortfolioAsync(portfolioModel, ct);

            _logger.LogInformation("Stock {Symbol} was successfully removed from portfolio of user {UserId}",
                symbolUpper, userID);
        }

        public async Task<List<StockPortfolioDto>> GetUserPortfolio(string userID, CancellationToken ct = default)
        {
            var portfolio = await _porrfolioRepo.GetUserPortfolio(userID, ct);

            return portfolio.Select(p => p.Stock.ToStockPortfolioDto()).ToList();
        }
    }
}
