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

        public async Task<List<Recipe>> searchByTitleOrDescriptionAsync(string filtro)
        {
            filtro = filtro.ToLower().Trim();

            return await _context.Recipes
            .Where(r => r.Enabled && (
                (r.Title != null && r.Title.ToLower().Contains(filtro)) ||
                (r.Description != null && r.Description.ToLower().Contains(filtro))
            )).ToListAsync();
        }

        public async Task<Recipe> UpdateAsync(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
            _context.Entry(recipe).Property(r => r.CreatedAt).IsModified = false;

            await _context.SaveChangesAsync();

            return recipe;
        }

        public async Task DeleteAsync(Recipe recipe)
        {
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
        }
    }
}