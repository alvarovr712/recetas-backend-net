using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId);
        Task<User> UpdateAsync(User user);

        Task<int> CountAllAsync();
        Task<int> CountCreatedSinceAsync(DateTime date);

        Task<int> CountCreatedBetweenAsync(DateTime start,DateTime end);
        
    }
}