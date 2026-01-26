using Web.API.Dtos.Stock;
using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface IPorrfolioRepository
    {
        Task<List<Portfolio>> GetUserPortfolio(string userID, CancellationToken ct = default);
        Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio, CancellationToken ct = default);
        Task DeletePortfolioAsync(Portfolio portfolio, CancellationToken ct = default);
        Task<bool> CheckIfExistsAsync(string userID, int stockID, CancellationToken ct = default);
        Task<Portfolio?> GetByIdAndSymbol(string userID, string symbolUpper, CancellationToken ct = default);
    }
}
