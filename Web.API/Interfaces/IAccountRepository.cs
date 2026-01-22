using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<AppUser>> GetAllAsync(CancellationToken ct = default);
    }
}
