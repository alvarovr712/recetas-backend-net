using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
namespace RecetasAPINet.Repositories
{


    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecetasDbContext _context;

        public RecipeRepository(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
        }

        public async Task<List<Recipe>> GetRecipesByUserIdAsync(Guid userId)
        {
            return await _context.Recipes
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }


        public async Task<Recipe?> GetByIdAsync(Guid id)
        {
            return await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Recipe>> GetAll()
        {
            return await _context.Recipes.ToListAsync();
        }

        public async Task<List<Recipe>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.Recipes
                .Where(r => ids.Contains(r.Id))
                .ToListAsync();
        }





    }
}