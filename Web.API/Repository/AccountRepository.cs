using Microsoft.EntityFrameworkCore;
using Web.API.Data;
using Web.API.Interfaces;
using Web.API.Models;

namespace Web.API.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDBContext _context;
        public AccountRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<AppUser>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Users.ToListAsync(ct);
        }
    }
}
