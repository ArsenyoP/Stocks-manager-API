using Microsoft.EntityFrameworkCore;
using Web.API.Dtos.Comment;
using Web.API.Dtos.Stock;
using Web.API.Exceptions;
using Web.API.Helpers;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using System.Diagnostics.SymbolStore;
using Web.API.Mappers;
using Web.API.Models;
using Web.API.Dtos.FMP;

namespace Web.API.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<StockService> _logger;
        private readonly IFinancialService _financialService;

        public StockService(IStockRepository stockRepository, ILogger<StockService> logger,
            IFinancialService financialService)
        {
            _stockRepository = stockRepository;
            _logger = logger;
            _financialService = financialService;
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

        public async Task<StockDto> Update(string symbol, UpdateStockRequestDto updateStock, CancellationToken ct)
        {
            var symbolUpper = updateStock.Symbol.ToUpper().Trim();
            var oldSymbolUpper = symbol.ToUpper().Trim();

            var stockModel = await _stockRepository.GetBySymbol(oldSymbolUpper, ct);

            if (stockModel == null)
            {
                _logger.LogWarning("Update stock failed: Stock with symbol {Symbol} not found", oldSymbolUpper);
                throw new KeyNotFoundException($"Stock with symbol {oldSymbolUpper} not found");
            }

            var existingWithSameSymbol = await _stockRepository.SymbolExists(symbolUpper, stockModel.ID, ct);

            if (existingWithSameSymbol)
            {
                _logger.LogWarning("Update stock failed: Symbol {Symbol} is already taken by another stock.", symbolUpper);
                throw new ArgumentException("Stock with this symbol already exists");
            }



            stockModel = await _stockRepository.UpdateAsync(stockModel, updateStock, ct);

            if (stockModel == null)
            {
                _logger.LogWarning("Update stock failed: Stock with symbol {Symbol} not found", symbolUpper);
                throw new KeyNotFoundException($"Stock with symbol {symbolUpper} not found");
            }

            _logger.LogInformation("Stock with symbol {Symbol} and ID {StockID} was successfully updated", symbolUpper, stockModel.ID);
            return stockModel.ToStockDto();
        }

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

        public async Task Delete(string symbol, CancellationToken ct)
        {
            var symbolUpper = symbol.ToUpper().Trim();
            var stockModel = await _stockRepository.GetBySymbol(symbolUpper, ct);

            if (stockModel == null)
            {
                _logger.LogWarning("Delete stock failed: Stock with symbol {Symbol} not found", symbolUpper);
                throw new KeyNotFoundException($"Stock with symbol {symbolUpper} not found");
            }
            await _stockRepository.DeleteAsync(stockModel, ct);

            _logger.LogInformation("Stock with symbol {Symbol} and ID {StockID} was successfully deleted", symbolUpper, stockModel.ID);
        }

        //methods for API tasks 
        public async Task<StockDto?> CreateFromApi(string symbol, CancellationToken ct)
        {
            var symbolUpper = symbol.ToUpper();
            var stock = await _financialService.GetFullStock(symbolUpper, ct);

            if (stock == null)
            {
                _logger.LogInformation("Can't create stock with symbol {symbolUpper}",
                    symbolUpper);
                return null;
            }

            stock = await _stockRepository.CreateAsync(stock, ct);

            if (stock == null)
            {
                _logger.LogError("Error while creating stock with symbol {symbolUpper}",
                    symbolUpper);
                return null;
            }

            return stock.ToStockDto();
        }

        public bool CheackIsFresh(Stock stock)
        {
            return stock.LastUpdate.AddMinutes(15) > DateTime.Now;
        }

        public async Task<StockDto?> RefreshStock(Stock stock, CancellationToken ct)
        {
            var stockSymbol = stock.Symbol;

            var refreshDto = await _financialService.GetRefreshedStockDto(stockSymbol, ct);

            if (refreshDto == null)
            {
                _logger.LogWarning("Can't refresh data for stock with symbol {symbol}",
                    stockSymbol);

                return stock.ToStockDto();
            }

            var updatedStock = await _stockRepository.RefreshPriceData(stock, refreshDto, ct);

            if (updatedStock == null)
            {
                _logger.LogError("Error while updating stock with symbol: {stockSymbol}", stockSymbol);
                throw new DbUpdateException($"Error while updating stock with symbol: {stockSymbol}");
            }

            return updatedStock.ToStockDto();
        }


        public async Task<StockDto?> GetBySymbol(string symbol, CancellationToken ct)
        {
            var symbolUpper = symbol.ToUpper();
            var stock = await _stockRepository.GetBySymbol(symbolUpper, ct);

            if (stock == null)
            {
                var stockDto = await CreateFromApi(symbolUpper, ct);

                if (stockDto == null)
                {
                    throw new NotFoundException("Can't find stock or you dont have access to it");
                }

                _logger.LogInformation("Added stock with symbol: {symbol}", symbolUpper);
                return stockDto;
            }

            if (!CheackIsFresh(stock))
            {
                var stockDto = await RefreshStock(stock, ct);
                _logger.LogInformation("Stock with symbol: {symbol} was updated", symbolUpper);
                return stockDto;
            }

            return stock.ToStockDto();
        }

        public async Task<StockDto> GetOrCreateStockAsync(string symbol, CancellationToken ct)
        {
            var symbolUpper = symbol.ToUpper();
            var stock = await _stockRepository.GetBySymbol(symbolUpper, ct);

            if (stock == null)
            {
                var stockDto = await CreateFromApi(symbolUpper, ct);
                if (stockDto == null)
                {
                    throw new NotFoundException("Can't find stock or you dont have access to it");
                }
                _logger.LogInformation("Added stock with symbol: {symbol}", symbolUpper);
                return stockDto;
            }
            return stock.ToStockDto();
        }
    }
}

