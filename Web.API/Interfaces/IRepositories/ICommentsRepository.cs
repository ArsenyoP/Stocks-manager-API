using Web.API.Dtos.Comment;
using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface ICommentsRepository
    {
        IQueryable<Comment> GetAllQuery(CancellationToken ct = default);
        Task<Comment?> GetById(int id, CancellationToken ct = default);
        Task<Comment?> CreateCommentAsync(Comment commentModel, CancellationToken ct = default);
        Task<Comment?> UpdateCommentAsync(Comment commentModel, UpdateCommentDto updateDto, CancellationToken ct = default);
        Task DeleteAsync(Comment commentModel, CancellationToken ct = default);
    }
}
