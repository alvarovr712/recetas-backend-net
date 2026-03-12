using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
namespace RecetasAPINet.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly RecetasDbContext _context;

        public IngredientRepository(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ingredient>> GetAllAsync()
        {
            return await _context.Ingredients.ToListAsync();
        }

        public async Task AddAsync(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Ingredient>> SearchIngredientsByNameAsync(string name)
        {
            name = name.ToLower().Trim();

            return await _context.Ingredients
                .Where(i => i.Name.ToLower().Contains(name))
                .ToListAsync();
        }

    }
}