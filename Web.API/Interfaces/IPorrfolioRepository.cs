using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface IPorrfolioRepository
    {
        Task<List<Stock>> GetUserPortfolio(AppUser user);
        Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio);
    }
}
