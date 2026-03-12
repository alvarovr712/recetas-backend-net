using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public interface ILogRepository
    {
        Task AddAsync(Log log);
        Task<List<Log>> GetByUserIdAsync(Guid userId);
        Task SaveChangesAsync();

        Task<int> CountRecipeCreationsAsync(Guid userId);
    }
}
