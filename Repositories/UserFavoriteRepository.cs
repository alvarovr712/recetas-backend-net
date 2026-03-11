
using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;
using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{

    public class UserFavoriteRepository : IUserFavoriteRepository
    {
        private readonly RecetasDbContext _context;

        public UserFavoriteRepository(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task<UserFavorite?> GetAsync(Guid userId, Guid recipeId)
        {
            return await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        }

        public async Task AddAsync(UserFavorite favorite)
        {
            await _context.UserFavorites.AddAsync(favorite);
        }

        public async Task RemoveAsync(UserFavorite favorite)
        {
            _context.UserFavorites.Remove(favorite);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserFavorite>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserFavorites
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }
    }
}