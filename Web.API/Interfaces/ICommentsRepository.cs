using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface ICommentsRepository
    {
        Task<List<Comment>> GetAllAsync(CancellationToken ct = default);
        Task<Comment?> GetById(int id, CancellationToken ct = default);
        Task<Comment?> CreateCommentAsync(Comment commentModel, CancellationToken ct = default);
        Task<Comment?> UpdateCommentAsync(int id, Comment commentModel, CancellationToken ct = default);
        Task<Comment?> DeleteAsync(int id, CancellationToken ct = default);
    }
}
