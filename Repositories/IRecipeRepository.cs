using RecetasAPINet.DTOs;
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
        
        Task<List<Recipe>> searchByTitleOrDescriptionAsync(string filtro);

        Task<Recipe> UpdateAsync(Recipe recipe);
        Task DeleteAsync(Recipe recipe);

        Task<int> CountAllAsync();
        Task<int> CountCreatedBetweenAsync(DateTime start, DateTime end);

        Task<List<ActiveUserDTO>> GetTopUsersByRecipesAsync(int top);

    }
}