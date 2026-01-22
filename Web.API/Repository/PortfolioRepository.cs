using Microsoft.EntityFrameworkCore;
using Web.API.Data;
using Web.API.Interfaces;
using Web.API.Models;

namespace Web.API.Repository
{
    public class PortfolioRepository : IPorrfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio, CancellationToken ct)
        {
            await _context.Portfolios.AddAsync(portfolio, ct);
            await _context.SaveChangesAsync(ct);
            return portfolio;
        }

        public async Task<Portfolio> DeletePortfolioAsync(AppUser user, string symbol, CancellationToken ct)
        {
            var portfolioModel = await _context.Portfolios.FirstOrDefaultAsync(
                s => s.AppUserId == user.Id && s.Stock.Symbol.ToLower() == symbol.ToLower(), ct);

            if (portfolioModel == null)
            {
                return null;
            }

            _context.Portfolios.Remove(portfolioModel);
            await _context.SaveChangesAsync(ct);
            return portfolioModel;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user, CancellationToken ct)
        {
            return await _context.Portfolios.Where(s => s.AppUserId == user.Id)
                .Select(stock => new Stock
                {
                    ID = stock.StockId,
                    Symbol = stock.Stock.Symbol,
                    CompanyName = stock.Stock.CompanyName,
                    Purchase = stock.Stock.Purchase,
                    LastDiv = stock.Stock.LastDiv,
                    Industy = stock.Stock.Industy,
                    MarketCap = stock.Stock.MarketCap
                }).ToListAsync(ct);
        }
    }
}
