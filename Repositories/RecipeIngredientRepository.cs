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
    }
}