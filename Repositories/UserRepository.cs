using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;
using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly RecetasDbContext _context;

        public UserRepository(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}