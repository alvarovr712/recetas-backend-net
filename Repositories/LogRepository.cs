using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;
using RecetasAPINet.DTOs;
using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly RecetasDbContext _context;

        public LogRepository(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Log log)
        {
            await _context.Logs.AddAsync(log);
        }

        public async Task<List<Log>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Logs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountRecipeCreationsAsync(Guid userId)
        {
            return await _context.Logs
                .Where(l => l.UserId == userId && l.Action == "CrearReceta")
                .CountAsync();
        }

        public async Task<List<DailyActivityDTO>> CountByDayAsync(DateTime start,DateTime end)
        {
            return await _context.Logs
            .Where(l => l.CreatedAt >= start && l.CreatedAt < end)
            .GroupBy(l => l.CreatedAt.Date)
            .Select (g => new DailyActivityDTO
            {
                Fecha = g.Key,
                Valor = g.Count()
            })
            .OrderBy(f => f.Fecha)
            .ToListAsync();
        }
    }
}
