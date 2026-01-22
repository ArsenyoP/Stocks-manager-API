using Microsoft.EntityFrameworkCore;
using Web.API.Data;
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

        public async Task<Comment?> CreateCommentAsync(Comment commentModel, CancellationToken ct)
        {
            await _context.Comments.AddAsync(commentModel, ct);
            await _context.SaveChangesAsync(ct);
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id, CancellationToken ct)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(s => s.ID == id, ct);

            if (comment == null)
            {
                return null;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync(ct);
            return comment;
        }

        public async Task<List<Comment>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Comments.Include(a => a.AppUser).ToListAsync(ct);
        }

        public Task<Comment?> GetById(int id, CancellationToken ct)
        {
            return _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(s => s.ID == id, ct);
        }

        public async Task<Comment?> UpdateCommentAsync(int id, Comment commentModel, CancellationToken ct)
        {
            var existingComment = await _context.Comments.FirstOrDefaultAsync(s => s.ID == id, ct);

            if (existingComment == null)
            {
                return null;
            }

            existingComment.Title = commentModel.Title;
            existingComment.Content = commentModel.Content;

            await _context.SaveChangesAsync(ct);
            return existingComment;
        }
    }
}
