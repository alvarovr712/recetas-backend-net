using RecetasAPINet.Models;
namespace RecetasAPINet.Repositories
{
    public interface IRecipeRepository
    {
        Task AddAsync(Recipe recipe);
        Task<List<Recipe>> GetRecipesByUserIdAsync(Guid userId);
        Task<Recipe?> GetByIdAsync(Guid id);
        Task<List<Recipe>> GetAll();

        Task<List<Recipe>> GetByIdsAsync(List<Guid> ids);

    }
}