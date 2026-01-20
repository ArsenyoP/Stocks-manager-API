using Web.API.Dtos.Stock;
using Web.API.Helpers;
using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync(QueryObject query);
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<Stock?> GetById(int id);
        Task<Stock?> CreateAsync(Stock stockModel);
        Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto);
        Task<Stock?> DeleteAsync(int id);
        Task<Stock?> BoostDividentsAsync(int id, decimal percent);
        Task<bool> StockExists(int id);
    }
}
