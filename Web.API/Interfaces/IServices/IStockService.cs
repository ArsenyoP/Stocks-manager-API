using Microsoft.AspNetCore.Mvc;
using Web.API.Dtos.FMP;
using Web.API.Dtos.Stock;
using Web.API.Helpers;
using Web.API.Models;

namespace Web.API.Interfaces.IServices
{
    public interface IStockService
    {
        public Task<List<StockDto>> GetAllAsync(QueryObject query, CancellationToken ct = default);
        public Task<StockDto?> GetById(int id, CancellationToken ct = default);
        public Task<StockDto> Create(CreateStockRequestDto stockDto, CancellationToken ct);
        public Task<StockDto> Update(int id, UpdateStockRequestDto updateStock, CancellationToken ct);
        public Task Delete(int id, CancellationToken ct);
        public Task<StockDto?> CreateFromApi(string symbol, CancellationToken ct);
        public bool CheackIsFresh(Stock stock);
        public Task<StockDto?> RefreshStock(Stock stock, CancellationToken ct);
        public Task<StockDto?> GetBySymbol(string symbol, CancellationToken ct);
    }
}
