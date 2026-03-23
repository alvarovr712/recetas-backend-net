
using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;
using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RecetasDbContext _context;

        private readonly ILogRepository _logRepository;

        public UserRepository(RecetasDbContext context, ILogRepository logRepository)
        {
            _context = context;
            _logRepository = logRepository;
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> UpdateAsync(User user)
        {
            
            _context.Entry(user).Property(u => u.CreatedAt).IsModified = false;
            
            // Fix UTC issue for PostgreSQL
            user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<int> CountAllAsync()
        {
            return _context.Users.CountAsync();
        }

        public Task<int> CountCreatedSinceAsync(DateTime date)
        {
            return _context.Users.CountAsync(u=> u.CreatedAt >= date);
        }

        public Task<int> CountCreatedBetweenAsync(DateTime start, DateTime end)
        {
            return _context.Users.CountAsync(u => u.CreatedAt >= start && u.CreatedAt < end);
        }





    }
}