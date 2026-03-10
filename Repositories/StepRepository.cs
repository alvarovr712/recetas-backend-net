using Microsoft.EntityFrameworkCore;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
namespace RecetasAPINet.Repositories
{


    public class StepRepository : IStepRepository
    {
        private readonly RecetasDbContext _context;

        public StepRepository(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<Step> steps)
        {
            await _context.Steps.AddRangeAsync(steps);
        }

        public async Task<List<Step>> GetByRecipeIdAsync(Guid recipeId)
        {
            return await _context.Steps
                .Where(s => s.RecipeId == recipeId)
                .OrderBy(s => s.StepOrder)
                .ToListAsync();
        }

    }
}