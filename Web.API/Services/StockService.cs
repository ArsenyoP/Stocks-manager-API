using Microsoft.EntityFrameworkCore;
using Web.API.Dtos.Comment;
using Web.API.Dtos.Stock;
using Web.API.Exceptions;
using Web.API.Helpers;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Exceptions;
using System.Diagnostics.SymbolStore;
using Web.API.Mappers;

namespace Web.API.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<StockService> _logger;

        public StockService(IStockRepository stockRepository, ILogger<StockService> logger)
        {
            _stockRepository = stockRepository;
            _logger = logger;
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
                        stockQuery = query.IsDescending
                            ? stockQuery.OrderByDescending(s => s.Symbol).ThenByDescending(x => x.ID)
                                : stockQuery.OrderBy(s => s.Symbol).ThenBy(x => x.ID);
                        break;
                    case "name":
                        stockQuery = query.IsDescending
                            ? stockQuery.OrderByDescending(s => s.CompanyName).ThenByDescending(x => x.ID)
                                : stockQuery.OrderBy(s => s.CompanyName).ThenBy(x => x.ID);
                        break;
                    case "dividends":
                        stockQuery = query.IsDescending
                            ? stockQuery.OrderByDescending(s => s.LastDiv).ThenByDescending(x => x.ID)
                                : stockQuery.OrderBy(s => s.LastDiv).ThenBy(x => x.ID);
                        break;
                    case "marcetprice":
                        stockQuery = query.IsDescending
                            ? stockQuery.OrderByDescending(s => s.MarketCap).ThenByDescending(x => x.ID)
                                : stockQuery.OrderBy(s => s.MarketCap).ThenBy(x => x.ID);
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

            var stockDto = await stockQuery
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
                            ID = c.ID,
                            Title = c.Title,
                            Content = c.Content,
                            CreatedBy = c.AppUser.UserName,
                            CreatedOn = c.CreatedOn
                        })
                        .ToList()
                }).FirstOrDefaultAsync(s => s.ID == id, ct);

            if (stockDto == null)
            {
                throw new NotFoundException($"Can't find stock with ID: {id}");
            }

            return stockDto;
        }

        public async Task<StockDto> Update(int id, UpdateStockRequestDto updateStock, CancellationToken ct)
        {
            var symbolUpper = updateStock.Symbol.ToUpper().Trim();
            var existingWithSameSymbol = await _stockRepository.SymbolExists(symbolUpper, id, ct);

            if (existingWithSameSymbol)
            {
                _logger.LogWarning("Update stock failed: Symbol {Symbol} is already taken by another stock.", symbolUpper);
                throw new ArgumentException("Stock with this symbol already exists");
            }

            var stockModel = await _stockRepository.UpdateAsync(id, updateStock, ct);

            if (stockModel == null)
            {
                _logger.LogWarning("Update stock failed: Stock with ID {StockId} not found", id);
                throw new KeyNotFoundException($"Stock with ID {id} not found");
            }

            _logger.LogInformation("Stock with ID {StockId} was successfully updated to symbol {Symbol}", id, symbolUpper);
            return stockModel.ToStockDto();
        }

        //check if stock exists
        public async Task<StockDto> Create(CreateStockRequestDto stockDto, CancellationToken ct)
        {
            bool symbolExists = await _stockRepository.SymbolExists(stockDto.Symbol, 0, ct);
            if (symbolExists)
            {
                _logger.LogWarning("Create stock failed: Symbol {Symbol} already exists", stockDto.Symbol);
                throw new ArgumentException($"Stock with symbol {stockDto.Symbol} already exists");
            }

            var stockModel = stockDto.ToStockFromCreateDTO();
            await _stockRepository.CreateAsync(stockModel, ct);

            _logger.LogInformation("New stock created: {Symbol} with ID {StockID}", stockModel.Symbol, stockModel.ID);
            return stockModel.ToStockDto();
        }

        public async Task Delete(int id, CancellationToken ct)
        {
            var stockModel = await _stockRepository.GetByIdAsync(id, ct);

            if (stockModel == null)
            {
                _logger.LogWarning("Delete stock failed: Stock with ID {StockId} not found", id);
                throw new KeyNotFoundException($"Can't find stock with ID: {id}");
            }
            await _stockRepository.DeleteAsync(stockModel, ct);

            _logger.LogInformation("Stock {Symbol} (ID: {StockId}) was successfully deleted", stockModel.Symbol, id);
        }

    }
}
