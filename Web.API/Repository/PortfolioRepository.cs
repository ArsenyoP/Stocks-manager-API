using Microsoft.EntityFrameworkCore;
using Web.API.Data;
using Web.API.Dtos.Stock;
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

        public async Task<bool> CheckIfExistsAsync(string userID, int stockID, CancellationToken ct)
        {
            return await _context.Portfolios
                .AnyAsync(p => p.StockId == stockID && p.AppUserId == userID, ct);
        }

        public async Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio, CancellationToken ct)
        {
            await _context.Portfolios.AddAsync(portfolio, ct);
            await _context.SaveChangesAsync(ct);
            return portfolio;
        }

        public async Task DeletePortfolioAsync(Portfolio portfolio, CancellationToken ct)
        {
            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Portfolio>> GetUserPortfolio(string userID, CancellationToken ct)
        {
            return await _context.Portfolios
                .Where(s => s.AppUserId == userID)
                .Include(s => s.Stock)
                .ToListAsync(ct);
        }

        public async Task<Portfolio?> GetByIdAndSymbol(string userID, string symbolUpper, CancellationToken ct)
        {
            return await _context.Portfolios.FirstOrDefaultAsync(
                s => s.AppUserId == userID && s.Stock.Symbol == symbolUpper, ct);
        }


    }
}
