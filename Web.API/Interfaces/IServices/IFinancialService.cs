using Web.API.Dtos.FMP;
using Web.API.Models;

namespace Web.API.Interfaces.IServices
{
    public interface IFinancialService
    {
        public Task<Stock?> GetFullStock(string symbol);
        public Task<FMPRenewDto[]> GetUpdatedStock(string symbol);
    }
}
