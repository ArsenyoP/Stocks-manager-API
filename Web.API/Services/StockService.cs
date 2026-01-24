using Microsoft.EntityFrameworkCore;
using Web.API.Dtos.Comment;
using Web.API.Dtos.Stock;
using Web.API.Exceptions;
using Web.API.Helpers;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Exceptions;
using System.Diagnostics.SymbolStore;

namespace Web.API.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<List<StockDto>> GetAllAsync(QueryObject query, CancellationToken ct = default)
        {
            var stockQuery = _stockRepository.GetAllQuery();

            if (query.Id.HasValue)
            {
                stockQuery = stockQuery.Where(s => s.ID == query.Id);
            }

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stockQuery = stockQuery.Where(s =>
                    s.CompanyName.ToLower().Contains(query.CompanyName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                var symbolUpper = query.Symbol.ToUpper();

                stockQuery = stockQuery.Where(s =>
                    s.Symbol.Contains(symbolUpper));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                var sortBy = query.SortBy?.ToLower().Trim();
                switch (sortBy)
                {
                    case "symbol":
                        stockQuery = query.IsDescending ? stockQuery.OrderByDescending(s => s.Symbol) : stockQuery.OrderBy(s => s.Symbol);
                        break;
                    case "name":
                        stockQuery = query.IsDescending ? stockQuery.OrderByDescending(s => s.CompanyName) : stockQuery.OrderBy(s => s.CompanyName);
                        break;
                    case "dividends":
                        stockQuery = query.IsDescending ? stockQuery.OrderByDescending(s => s.LastDiv) : stockQuery.OrderBy(s => s.LastDiv);
                        break;
                    case "marcetprice":
                        stockQuery = query.IsDescending ? stockQuery.OrderByDescending(s => s.MarketCap) : stockQuery.OrderBy(s => s.MarketCap);
                        break;
                }
            }
            else
            {
                stockQuery = query.IsDescending ? stockQuery.OrderByDescending(s => s.ID) : stockQuery.OrderBy(s => s.ID);
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stockQuery
                .Skip(skipNumber)
                .Take(query.PageSize)
                .Select(s => new StockDto
                {
                    ID = s.ID,
                    CompanyName = s.CompanyName,
                    Symbol = s.Symbol,
                    Purchase = s.Purchase,
                    LastDiv = s.LastDiv,
                    Industy = s.Industy,
                    MarketCap = s.MarketCap,
                    Comments = s.Comments.OrderByDescending(t => t.CreatedOn)
                        .Take(10)
                        .Select(c => new CommentDto
                        {
                            ID = c.ID,
                            Title = c.Title,
                            Content = c.Content,
                            CreatedOn = c.CreatedOn,
                            CreatedBy = c.AppUser.UserName ?? "Anonymous"
                        }).ToList()
                })
                .ToListAsync(ct);
        }

        public async Task<StockDto?> GetById(int id, CancellationToken ct = default)
        {
            var stockQuery = _stockRepository.GetAllQuery();

            var stock = await stockQuery.FirstOrDefaultAsync(s => s.ID == id, ct);

            if (stock == null)
            {
                throw new NotFoundException($"Can't find stock with ID: {id}");
            }

            return await stockQuery
                .Select(s => new StockDto
                {
                    ID = s.ID,
                    CompanyName = s.CompanyName,
                    Symbol = s.Symbol,
                    Purchase = s.Purchase,
                    LastDiv = s.LastDiv,
                    Industy = s.Industy,
                    MarketCap = s.MarketCap,
                    Comments = s.Comments
                        .OrderByDescending(t => t.CreatedOn)
                        .Take(20)
                        .Select(c => new CommentDto
                        {
                            ID = s.ID,
                            Title = c.Title,
                            Content = c.Content,
                            CreatedBy = c.AppUser.UserName,
                            CreatedOn = c.CreatedOn
                        })
                        .ToList()
                }).FirstOrDefaultAsync(s => s.ID == id, ct);
        }
    }
}
