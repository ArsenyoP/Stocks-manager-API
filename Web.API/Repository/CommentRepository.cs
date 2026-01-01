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

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public Task<Comment?> GetById(int id)
        {
            return _context.Comments.FirstOrDefaultAsync(s => s.ID == id);
        }
    }
}
