using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;

namespace RecetasAPINet.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly RecetasDbContext _context;

        public SessionRepository(RecetasDbContext context)
        {
            _context = context;
        }

        public Task<int> CountAllAsync()
        {
            return _context.Sessions.CountAsync(s => s.Enabled == true);
        }

        public Task<int> CountCreatedBetweenAsync(DateTime start, DateTime end)
        {
            return _context.Sessions.CountAsync(s => s.CreatedAt >= start && s.CreatedAt <end);
        }
    }

   
}