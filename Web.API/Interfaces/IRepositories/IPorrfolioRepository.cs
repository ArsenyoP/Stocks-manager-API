using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface IPorrfolioRepository
    {
        Task<List<Stock>> GetUserPortfolio(AppUser user, CancellationToken ct = default);
        Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio, CancellationToken ct = default);
        Task<Portfolio> DeletePortfolioAsync(AppUser user, string symbol, CancellationToken ct = default);
    }
}
