using Web.API.Dtos.FMP;
using Web.API.Models;

namespace Web.API.Interfaces.IServices
{
    public interface IFinancialService
    {
        public Task<Stock?> GetFullStock(string symbol, CancellationToken ct);
        public Task<FMPRefreshDto?> GetRefreshedStockDto(string symbol, CancellationToken ct);
    }
}
