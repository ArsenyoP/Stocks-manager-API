using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using Web.API.Data;
using Web.API.Dtos.Stock;
using Web.API.Helpers;
using Web.API.Interfaces;
using Web.API.Models;
using Web.API.Dtos.Comment;
using Web.API.Dtos.FMP;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.API.Repository
{
    public class StockRepository : IStockRepository
    {

        private readonly ApplicationDBContext _contex;
        public StockRepository(ApplicationDBContext context)
        {
            _contex = context;
        }

        public IQueryable<Stock> GetAllQuery()
        {
            var stocksQuery = _contex.Stocks.AsNoTracking();

            return stocksQuery;
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

        public async Task<Stock?> DeleteAsync(Stock stock, CancellationToken ct)
        {
            _contex.Stocks.Remove(stock);
            await _contex.SaveChangesAsync(ct);
            return stock;
        }

        public async Task<int> GetIdBySymbolAsync(string symbol, CancellationToken ct)
        {
            return await _contex.Stocks
                .Where(s => s.Symbol == symbol)
                .Select(s => s.ID)
                .FirstOrDefaultAsync(ct);
        }
        public Task<bool> StockExists(string symbol, CancellationToken ct)
        {
            var symbolUpper = symbol.ToUpper();
            return _contex.Stocks.AnyAsync(s => s.Symbol == symbolUpper, ct);
        }

        public async Task<bool> SymbolExists(string symbol, int currentId, CancellationToken ct)
        {
            return await _contex.Stocks
                .AnyAsync(s => s.Symbol == symbol && s.ID != currentId, ct);
        }

        public async Task<Stock?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _contex.Stocks
                .Include(x => x.Comments)
                .ThenInclude(c => c.AppUser)
                .FirstOrDefaultAsync(s => s.ID == id, ct);
        }

        public async Task<Stock?> RefreshPriceData(Stock stock, FMPRefreshDto refreshDto, CancellationToken ct)
        {
            if (stock == null)
            {
                return null;
            }

            stock.Purchase = refreshDto.Price;
            stock.LastDiv = refreshDto.Dividend;
            stock.MarketCap = (long)Math.Round(refreshDto.MarketCap, 0, MidpointRounding.AwayFromZero);

            stock.LastUpdate = DateTime.Now;
            stock.UpdateCount++;

            await _contex.SaveChangesAsync(ct);
            return stock;
        }

        public async Task<Stock?> GetBySymbol(string symbol, CancellationToken ct = default)
        {
            return await _contex.Stocks
                .Include(x => x.Comments)
                .ThenInclude(c => c.AppUser)
                .FirstOrDefaultAsync(x => x.Symbol == symbol, ct);
        }
    }
}
