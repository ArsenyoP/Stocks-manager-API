using Web.API.Dtos.Stock;
using Web.API.Helpers;
using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface IStockRepository
    {
        IQueryable<Stock> GetAllQuery();
        Task<Stock?> CreateAsync(Stock stockModel, CancellationToken ct = default);
        Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto, CancellationToken ct = default);
        Task<Stock?> DeleteAsync(int id, CancellationToken ct = default);
        Task<Stock?> BoostDividentsAsync(int id, decimal percent, CancellationToken ct = default);
        Task<bool> StockExists(int id, CancellationToken ct = default);
        Task<int> GetIdBySymbolAsync(string symbol, CancellationToken ct = default);

    }
}
