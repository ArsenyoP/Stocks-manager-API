using Web.API.Models;

namespace Web.API.Interfaces.IServices
{
    public interface IPorfolioService
    {
        Task<Portfolio> AddToPortfolio(string symbol, string userID, CancellationToken ct = default);
        Task DeletePortfolioAsync(string symbol, string userID, CancellationToken ct = default);
    }
}
