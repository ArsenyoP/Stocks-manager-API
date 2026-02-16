using Web.API.Dtos.FMP;
using Web.API.Dtos.Stock;
using Web.API.Helpers;
using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface IStockRepository
    {
        IQueryable<Stock> GetAllQuery();
        Task<Stock?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Stock?> GetBySymbol(string symbol, CancellationToken ct = default);
        Task<int> GetIdBySymbolAsync(string symbol, CancellationToken ct = default);


        Task<Stock?> CreateAsync(Stock stockModel, CancellationToken ct = default);
        Task<Stock?> UpdateAsync(Stock stock, UpdateStockRequestDto stockDto, CancellationToken ct = default);
        Task<Stock?> DeleteAsync(Stock stock, CancellationToken ct = default);

        Task<Stock?> RefreshPriceData(Stock stock, FMPRefreshDto refreshDto, CancellationToken ct = default);

        Task<bool> StockExists(string symbol, CancellationToken ct = default);
        Task<bool> SymbolExists(string symbol, int currentId, CancellationToken ct = default);
    }
}
