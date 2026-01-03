using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface ICommentsRepository
    {
        Task<List<Comment>> GetAllAsync();
        Task<Comment?> GetById(int id);
        Task<Comment?> CreateCommentAsync(Comment commentModel);
        Task<Comment?> UpdateCommentAsync(int id, Comment commentModel);
    }
}
