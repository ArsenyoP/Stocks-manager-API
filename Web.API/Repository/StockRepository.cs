using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using Web.API.Data;
using Web.API.Dtos.Stock;
using Web.API.Helpers;
using Web.API.Interfaces;
using Web.API.Models;
using Web.API.Dtos.Comment;

namespace Web.API.Repository
{
    public class StockRepository : IStockRepository
    {

        private readonly ApplicationDBContext _contex;
        public StockRepository(ApplicationDBContext context)
        {
            _contex = context;
        }

        public async Task<List<StockDto>> GetAllAsync(QueryObject query, CancellationToken ct)
        {
            var stocksQuery = _contex.Stocks.AsNoTracking();

            if (query.Id.HasValue)
            {
                stocksQuery = stocksQuery.Where(s => s.ID == query.Id);
            }

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocksQuery = stocksQuery.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocksQuery = stocksQuery.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocksQuery = query.IsDescending ? stocksQuery.OrderByDescending(s => s.Symbol) : stocksQuery.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;


            return await stocksQuery
                .OrderBy(s => s.ID)
                .Skip(skipNumber)
                .Take(query.PageSize)
                .Select(s => new StockDto
                {
                    ID = s.ID,
                    CompanyName = s.CompanyName,
                    Symbol = s.Symbol,
                    Industy = s.Industy,
                    LastDiv = s.LastDiv,
                    MarketCap = s.MarketCap,
                    Purchase = s.Purchase,
                    Comments = s.Comments.OrderByDescending(t => t.CreatedOn)
                    .Take(10).
                    Select(c => new CommentDto
                    {
                        ID = c.ID,
                        Title = c.Title,
                        Content = c.Content,
                        CreatedOn = c.CreatedOn,
                        CreatedBy = c.AppUser.UserName
                    }).ToList()
                }).ToListAsync(ct);
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
