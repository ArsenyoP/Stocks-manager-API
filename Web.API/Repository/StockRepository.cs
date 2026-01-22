using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using Web.API.Data;
using Web.API.Dtos.Stock;
using Web.API.Helpers;
using Web.API.Interfaces;
using Web.API.Models;

namespace Web.API.Repository
{
    public class StockRepository : IStockRepository
    {

        private readonly ApplicationDBContext _contex;
        public StockRepository(ApplicationDBContext context)
        {
            _contex = context;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query, CancellationToken ct)
        {
            var stocks = _contex.Stocks.Include(s => s.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if (query.Id.HasValue)
            {
                stocks = stocks.Where(s => s.ID == query.Id);
            }

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;


            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync(ct);
        }

        public async Task<Stock?> CreateAsync(Stock stockModel, CancellationToken ct)
        {
            await _contex.Stocks.AddAsync(stockModel, ct);
            await _contex.SaveChangesAsync(ct);
            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto, CancellationToken ct)
        {
            var exitingStock = await _contex.Stocks.FirstOrDefaultAsync(s => s.ID == id, ct);

            if (exitingStock == null)
            {
                return null;
            }

            exitingStock.Symbol = stockDto.Symbol;
            exitingStock.CompanyName = stockDto.CompanyName;
            exitingStock.MarketCap = stockDto.MarketCap;
            exitingStock.Purchase = stockDto.Purchase;
            exitingStock.Industy = stockDto.Industy;
            exitingStock.LastDiv = stockDto.LastDiv;

            await _contex.SaveChangesAsync(ct);
            return exitingStock;
        }

        public async Task<Stock?> DeleteAsync(int id, CancellationToken ct)
        {
            var stockModel = await _contex.Stocks.FirstOrDefaultAsync(s => s.ID == id, ct);

            if (stockModel == null)
            {
                return null;
            }

            _contex.Stocks.Remove(stockModel);
            await _contex.SaveChangesAsync(ct);
            return stockModel;
        }

        public async Task<Stock?> BoostDividentsAsync(int id, decimal percent, CancellationToken ct)
        {
            var stockModel = await _contex.Stocks.FirstOrDefaultAsync(x => x.ID == id, ct);

            if (stockModel == null)
            {
                return null;
            }

            decimal updatedDevidents = stockModel.LastDiv * (percent / 100);
            stockModel.LastDiv = stockModel.LastDiv += updatedDevidents;

            await _contex.SaveChangesAsync(ct);
            return stockModel;
        }

        public Task<bool> StockExists(int id, CancellationToken ct)
        {
            return _contex.Stocks.AnyAsync(s => s.ID == id, ct);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol, CancellationToken ct)
        {
            return await _contex.Stocks.Include(c => c.Comments).ThenInclude(a => a.AppUser).
                FirstOrDefaultAsync(s => s.Symbol.ToLower() == symbol.ToLower(), ct);
        }

        public async Task<Stock?> GetById(int id, CancellationToken ct)
        {
            return await _contex.Stocks.Include(c => c.Comments).ThenInclude(a => a.AppUser)
                .FirstOrDefaultAsync(s => s.ID == id, ct);
        }
    }
}
