using Web.API.Dtos.Stock;
using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync();
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<Stock?> CreateAsync(Stock stockModel);
        Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto);
        Task<Stock?> DeleteAsync(int id);
        Task<List<Stock>?> GetThreeGighrstDivedentsAsync();
        Task<Stock?> GetTopAsync();
        Task<Stock?> CreateLightAsync(Stock stockModel);
        Task<Stock?> UpdateSymbolAsync(int id, string symbol);
        Task<Stock?> BoostDividentsAsync(int id, decimal percent);
        Task<Stock?> SecureUpdateAsync(int id, SecureUpdateDTO updateDto);
        Task<bool> StockExists(int id);
    }
}
