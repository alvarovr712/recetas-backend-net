using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public interface IStepRepository
    {
        Task AddRangeAsync(List<Step> steps);
        Task<List<Step>> GetByRecipeIdAsync(Guid recipeId);

    }
}