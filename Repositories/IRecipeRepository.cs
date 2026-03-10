using RecetasAPINet.Models;
namespace RecetasAPINet.Repositories
{
    public interface IRecipeRepository
    {
        Task AddAsync(Recipe recipe);
    }
}