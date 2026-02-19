using Web.API.Dtos.Comment;
using Web.API.Models;

namespace Web.API.Interfaces.IServices
{
    public interface ICommentService
    {
        public Task<List<CommentDto>> GetAll(int page, CancellationToken ct = default);
        public Task<CommentDto> GetById(int id, CancellationToken ct);
        public Task<CommentDto> CreateComment(string symbol, string AppUserId, CreateCommentDto commentDto, CancellationToken ct);
        public Task<CommentDto> UpdateComment(int id, UpdateCommentDto updateDto, CancellationToken ct);
        public Task DeleteComment(int id, CancellationToken ct);

    }
}
