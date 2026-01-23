using Web.API.Dtos.Stock;
using Web.API.Helpers;

namespace Web.API.Interfaces.IServices
{
    public interface IStockService
    {
        public Task<List<StockDto>> GetAllAsync(QueryObject query, CancellationToken ct = default);
        public Task<StockDto> GetById(int id, CancellationToken ct = default);
        public Task<StockDto> GetBySymbol(string symbol, CancellationToken ct = default);
    }
}
