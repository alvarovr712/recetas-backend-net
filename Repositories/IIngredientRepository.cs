using RecetasAPINet.Models;

namespace RecetasAPINet.Repositories
{
    public interface IIngredientRepository
    {
        Task<List<Ingredient>> GetAllAsync();
        Task AddAsync(Ingredient ingredient);
    }
}