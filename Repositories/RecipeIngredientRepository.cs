using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;
using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{

    public class RecipeIngredientRepository : IRecipeIngredientRepository
    {
        private readonly RecetasDbContext _context;

        public RecipeIngredientRepository(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<RecipeIngredient> ingredients)
        {
            await _context.RecipeIngredients.AddRangeAsync(ingredients);
        }

        public async Task<List<RecipeIngredient>> GetByRecipeIdAsync(Guid recipeId)
        {
            return await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .Where(ri => ri.RecipeId == recipeId)
                .ToListAsync();
        }

    }
}