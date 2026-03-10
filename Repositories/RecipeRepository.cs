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

        public async Task<List<RecipeCardDto>> GetRecipesByUserIdAsync(Guid userId)
        {
            return await _context.Recipes
            .Where(r => r.UserId == userId)
            .Select(r => new RecipeCardDto
            {
                Id = r.Id,
                Image = r.Image,
                Title = r.Title,
                Description = r.Description,
                Type = r.Type
            }).ToListAsync();
        }

        public async Task<Recipe?> GetByIdAsync(Guid id)
        {
            return await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id);
        }

    }
}