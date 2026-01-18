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

        public async Task<Comment?> CreateCommentAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(s => s.ID == id);

            if (comment == null)
            {
                return null;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.Include(a => a.AppUser).ToListAsync();
        }

        public Task<Comment?> GetById(int id)
        {
            return _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(s => s.ID == id);
        }

        public async Task<Comment?> UpdateCommentAsync(int id, Comment commentModel)
        {
            var existingComment = await _context.Comments.FirstOrDefaultAsync(s => s.ID == id);

            if (existingComment == null)
            {
                return null;
            }

            existingComment.Title = commentModel.Title;
            existingComment.Content = commentModel.Content;

            await _context.SaveChangesAsync();
            return existingComment;
        }
    }
}
