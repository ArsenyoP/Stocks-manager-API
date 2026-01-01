using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface ICommentsRepository
    {
        Task<List<Comment>> GetAllAsync();
        Task<Comment?> GetById(int id);
    }
}
