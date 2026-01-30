using Microsoft.EntityFrameworkCore;
using Web.API.Data;
using Web.API.Dtos.Comment;
using Web.API.Interfaces;
using Web.API.Models;

namespace Web.API.Repository
{
    public class CommentRepository : ICommentsRepository
    {
        private readonly ApplicationDBContext _context;
        public CommentRepository(ApplicationDBContext contex)
        {
            _context = contex;
        }

        public IQueryable<Comment> GetAllQuery(CancellationToken ct)
        {
            return _context.Comments.AsNoTracking();
        }

        public async Task<Comment?> CreateCommentAsync(Comment commentModel, CancellationToken ct)
        {
            await _context.Comments.AddAsync(commentModel, ct);
            await _context.SaveChangesAsync(ct);
            return commentModel;
        }

        public async Task DeleteAsync(Comment commentModel, CancellationToken ct)
        {
            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync(ct);
        }

        public Task<Comment?> GetById(int id, CancellationToken ct)
        {
            return _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(s => s.ID == id, ct);
        }

        public async Task<Comment?> UpdateCommentAsync(Comment commentModel, UpdateCommentDto updateDto, CancellationToken ct)
        {
            commentModel.Title = updateDto.Title;
            commentModel.Content = updateDto.Content;

            await _context.SaveChangesAsync(ct);
            return commentModel;
        }
    }
}
