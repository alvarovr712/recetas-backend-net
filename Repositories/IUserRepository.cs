using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId);
    }
}