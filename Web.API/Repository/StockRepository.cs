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

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _contex.Stocks.Include(s => s.Comments).ThenInclude(a => a.AppUser).AsQueryable();

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


            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public Task<Stock?> GetByIdAsync(int id)
        {
            return _contex.Stocks.Include(s => s.Comments).FirstOrDefaultAsync(s => s.ID == id);
        }

        public Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return _contex.Stocks.Include(s => s.Comments).FirstOrDefaultAsync(s => s.Symbol == symbol);
        }

        public async Task<Stock?> CreateAsync(Stock stockModel)
        {
            await _contex.Stocks.AddAsync(stockModel);
            await _contex.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var exitingStock = await _contex.Stocks.FirstOrDefaultAsync(s => s.ID == id);

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

            await _contex.SaveChangesAsync();
            return exitingStock;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _contex.Stocks.FirstOrDefaultAsync(s => s.ID == id);

            if (stockModel == null)
            {
                return null;
            }

            _contex.Stocks.Remove(stockModel);
            await _contex.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>?> GetThreeGighrstDivedentsAsync()
        {
            var threeHightDivs = await _contex.Stocks.Include(s => s.Comments).OrderByDescending(s => s.LastDiv)
                .Take(3).ToListAsync();
            if (threeHightDivs == null)
            {
                return null;
            }

            return threeHightDivs;
        }

        public async Task<Stock?> GetTopAsync()
        {
            var topStock = await _contex.Stocks.Include(s => s.Comments).OrderByDescending(s => s.Purchase)
                .FirstOrDefaultAsync();
            if (topStock == null)
            {
                return null;
            }

            return topStock;
        }


        public async Task<Stock?> UpdateSymbolAsync(int id, string symbol)
        {
            var stockModel = await _contex.Stocks.FirstOrDefaultAsync(x => x.ID == id);

            if (stockModel == null)
            {
                return null;
            }

            stockModel.Symbol = symbol;
            await _contex.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> BoostDividentsAsync(int id, decimal percent)
        {
            var stockModel = await _contex.Stocks.FirstOrDefaultAsync(x => x.ID == id);

            if (stockModel == null)
            {
                return null;
            }

            decimal updatedDevidents = stockModel.LastDiv * (percent / 100);
            stockModel.LastDiv = stockModel.LastDiv += updatedDevidents;

            await _contex.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> SecureUpdateAsync(int id, SecureUpdateDTO updateDto)
        {
            var stockModel = await _contex.Stocks.FirstOrDefaultAsync(x => x.ID == id);

            if (stockModel == null)
            {
                return null;
            }

            stockModel.CompanyName = updateDto.CompanyName;
            stockModel.Purchase = updateDto.Purchase;

            await _contex.SaveChangesAsync();
            return stockModel;
        }


        public async Task<Stock?> CreateLightAsync(Stock stockModel)
        {
            await _contex.Stocks.AddAsync(stockModel);
            await _contex.SaveChangesAsync();
            return stockModel;
        }

        public Task<bool> StockExists(int id)
        {
            return _contex.Stocks.AnyAsync(s => s.ID == id);
        }
    }
}
