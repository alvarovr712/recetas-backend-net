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
    }
}