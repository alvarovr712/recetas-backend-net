using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public interface IUserFavoriteRepository
    {
        Task<UserFavorite?> GetAsync(Guid userId, Guid recipeId);
        Task AddAsync(UserFavorite favorite);
        Task RemoveAsync(UserFavorite favorite);
        Task SaveChangesAsync();

        Task<List<UserFavorite>> GetByUserIdAsync(Guid userId);
    }
}